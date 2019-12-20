open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Net
open System.Text.RegularExpressions
#r "bin/Debug/netcoreapp2.2/WebCrawling.dll"
open FSharp.Data

module Helpers =

    let follow_links url =
        let doc = HtmlDocument.Load(url:string)
        let linksList = 
            doc.Descendants ["a"]
            |> Seq.choose (fun x -> 
                   x.TryGetAttribute("href")
                   |> Option.map (fun a -> x.InnerText(), a.Value())
            )
        linksList
