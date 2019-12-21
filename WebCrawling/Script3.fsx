#r "C:/Users/Jesper/.nuget/packages/fsharp.data/3.3.2/lib/net45/FSharp.Data.dll"
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Net
open System.Text.RegularExpressions
open FSharp.Data

module Helpers =

    

    let rec extractRelevantLinks list =
        let pattern2 = "(?i)^https?"
        printfn "%A" list
        list |> List.filter (fun x -> Regex(pattern2).IsMatch(x))

    let follow_links url =
        let doc = HtmlDocument.Load(url:string)
        let linksList = 
            doc.Descendants ["a"]
            |> Seq.choose (fun x -> 
                   x.TryGetAttribute("href")
                   |> Option.map (fun a -> a.Value())
                   

            )
        let newList:string list = linksList |> List.ofSeq
        extractRelevantLinks newList
        

    
    follow_links "https://www.google.dk/search?q=hund"