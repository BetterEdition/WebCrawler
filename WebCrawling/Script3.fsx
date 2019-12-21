#I "/Users/edwinsilva/.nuget/packages/fsharp.data/3.3.2/lib/net45/"
#r "FSharp.Data.dll"
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Net
open System.Text.RegularExpressions
open FSharp.Data

module Helpers =

    let image_scrapper (urlLinks, originSite:string)  =
       //printfn "list %A" list
        let newUrls = urlLinks |> List.map(fun a-> "https://"+originSite+a)
        for url in newUrls do
            printf "%s" url
            let doc = HtmlDocument.Load(url:string)
            let images = 
                doc.Descendants ["img"]
                |> Seq.choose (fun x -> 
                       x.TryGetAttribute("src")
                       |> Option.map (fun a -> a.Value())
                )
            printfn "%A" images

            

    let rec extractRelevantLinks list  =
        let pattern2 = "(?i)^https://|^http://"
       //printfn "list %A" list
        list |> List.filter (fun x -> Regex(pattern2).IsMatch(x))  

    let follow_links url =
        let doc = HtmlDocument.Load(url:string)
        let linksList = 
            doc.Descendants ["a"]
            |> Seq.choose (fun x -> 
                   x.TryGetAttribute("href")
                   |> Option.map (fun a -> a.Value())
            )
        let newList = linksList |> List.ofSeq
        //printfn "newList %A" newList 

        let originURL = url.Split "/"
        let originPath =  originURL.[2]

        let canBeCrawled = extractRelevantLinks newList 
        image_scrapper (newList, originPath)

        //printfn "canBeCrawled %A" canBeCrawled 

        canBeCrawled

       


   


    follow_links "https://www.guloggratis.dk/dyr/hunde/hunde/"