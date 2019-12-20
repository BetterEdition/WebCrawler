#r "C:/Users/Jesper/.nuget/packages/fsharp.data/3.3.2/lib/net45/FSharp.Data.dll"
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Net
open System.Text.RegularExpressions
open FSharp.Data

module Helpers =

    let rec fix_href_links links = 
        match links with 
        | [] -> links
        | x::xs -> 
    

    let fix_href_links2 links =
           let pattern1 = "(?i)href\\s*=\\s*(\"|\')/?((?!#.*|/\B|" + 
                          "mailto:|location\.|javascript:)[^\"\']+)(\"|\')"
           let pattern2 = "(?i)^https?"
    
           let links =
               [
                   for x in Regex(pattern1).Matches(links) do
                       yield x.Groups.[2].Value
               ]
               |> List.filter (fun x -> Regex(pattern2).IsMatch(x))
           links


    let follow_links url =
        let doc = HtmlDocument.Load(url:string)
        let linksList = 
            doc.Descendants ["a"]
            |> Seq.choose (fun x -> 
                   x.TryGetAttribute("href")
                   |> Option.map (fun a -> a.Value())
            )
        fix_href_links linksList

    
    follow_links "https://www.google.dk/search?q=hund"

