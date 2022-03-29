module Json

open Giraffe
open Microsoft.AspNetCore.Http

/// <summary>
/// Sends JSON formatted text as application/json 
/// </summary>
/// <param name="str">JSON formatted text</param>
let text (str : string) : HttpHandler =
        let bytes = System.Text.Encoding.UTF8.GetBytes str
        fun (_ : HttpFunc) (ctx : HttpContext) ->
            ctx.SetContentType "application/json; charset=utf-8"
            ctx.WriteBytesAsync bytes
