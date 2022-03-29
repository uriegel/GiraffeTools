module GiraffeTools

open Giraffe
open Microsoft.AspNetCore.Http
open System.Threading.Tasks

/// <summary>
/// Try to route all pathes, a function is called with path as parameter. Use in combination with <see cref="httpHandlerParam" />
/// <code>
///    routePathes () <| httpHandlerParam getResourceFile 
/// </code>
/// </summary>
/// <param name="()">empty (unit)</param>
let routePathes () (routeHandler : string -> HttpHandler) : HttpHandler =
    fun (next : HttpFunc) (ctx : HttpContext) ->
        Some (SubRouting.getNextPartOfPath ctx)
        |> function
            | Some subpath -> routeHandler subpath[1..] next ctx    
            | None         -> skipPipeline

/// <summary>
/// Calls an HTTPHandler with a parameter. Is used in combination with <see cref="routePathes" />
/// <code>
///    routePathes () <| httpHandlerParam getResourceFile 
/// </code>
/// </summary>
let httpHandlerParam httpHandler param: HttpHandler = (fun () -> httpHandler(param))()

/// <summary>
/// Skips the pipeline in an asynchronous context
/// </summary>
let skip (_: HttpFunc) (__: HttpContext) = Task.FromResult None



