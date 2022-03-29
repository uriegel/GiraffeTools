module Sse

open Giraffe
open Microsoft.AspNetCore.Http
open System
open System.Text.Json
open System.Threading.Tasks

/// <summary>
/// Creates an 'Server Send Events' sender, fires in combination with an observable, uses JSON formatted messages
/// </summary>
/// <param name="observable">Observable of some type 'a to fire msg</param>
/// <param name="jsonOptions">JsonSerializerOptions to be used by System.Text.Json JSON serializing</param>
let create<'a> (observable: IObservable<'a>) (jsonOptions: JsonSerializerOptions) =  

    fun (next : HttpFunc) (ctx : HttpContext) ->
        task {
            let res = ctx.Response
            ctx.SetStatusCode 200
            ctx.SetHttpHeader("Content-Type", "text/event-stream")
            ctx.SetHttpHeader("Cache-Control", "no-cache")

            let messageLoop (inbox: MailboxProcessor<obj>) = 
                let rec messageLoop () = async {
                    let! msg = inbox.Receive()
                    let payload = JsonSerializer.Serialize(msg, jsonOptions)
                    do! res.WriteAsync $"data:{payload}\n\n" |> Async.AwaitTask
                    do! res.Body.FlushAsync() |> Async.AwaitTask
                    return! messageLoop ()
                }
                messageLoop ()
            
            let agent = MailboxProcessor.Start messageLoop
            let onValue evt = agent.Post evt
            observable |> Observable.subscribe onValue |> ignore

            // Wait forever
            let tcs = TaskCompletionSource<'a>()
            let! _ = tcs.Task
            // Only needed for compiling:
            return! text "" next ctx            
        }        

