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

    let image_scrapper urlLinks   =
        printfn "imageScraper"
        let pattern1 = "(?i)^https://|^http://"
        for url in urlLinks do
            printf "url :%s" url
            let doc = HtmlDocument.Load(url:string)
            let images = 
                doc.Descendants ["img"]
                |> Seq.choose (fun x -> 
                       x.TryGetAttribute("src")
                       |> Option.map (fun a -> a.Value())

                )
            let originPath = url.Split("/") 
            let origin =  originPath.[0] + "//" + originPath.[2]
            printfn "origin :%s" origin
            let correctImages = images |> List.ofSeq |> List.filter(fun x -> Regex(pattern1).IsMatch(x) = false) |> List.map(fun s -> origin + s)
               // |> Seq.filter(fun x -> Regex(pattern1).IsMatch(x) = false) |> Seq.map(fun x -> url + x)
            printfn "images: %A " correctImages
            File.AppendAllLines("/Users/edwinsilva/Desktop/WebCrawler/WebCrawling/ImageLinks.txt", correctImages)

            
            

            

    let rec extractRelevantLinks (list: string list, originpath)  =
        let pattern1 = "(?i)^https://|^http://|^#|^j"
        let pattern2 = "(?i)^https://|^http://"



        let subLinks = list |> List.filter (fun x -> Regex(pattern1).IsMatch(x) = false) |> List.map(fun x -> originpath + x)
        let mainLinks = list |> List.filter (fun x -> Regex(pattern2).IsMatch(x))
        mainLinks @ subLinks

    let mutable visited = new List<string>()
    
    let rec follow_links urlLinks =

        match urlLinks with
        | [] -> printfn "visited: %i"visited.Count
        | x::xs ->
        if visited.Contains(x) = false then
            visited.Add(x)
            let doc = HtmlDocument.Load(x:string)
            let linksList = 
                doc.Descendants ["a"]
                |> Seq.choose (fun x -> 
                       x.TryGetAttribute("href")
                       |> Option.map (fun a -> a.Value())
                )
            let newList = linksList |> List.ofSeq

            let originURL = x.Split "/"
            let originPath = originURL.[0] + "//"+ originURL.[2]

            let canBeCrawled = extractRelevantLinks (newList, originPath)
            image_scrapper canBeCrawled
            follow_links canBeCrawled
        else 
            follow_links xs



       


   


    follow_links ["https://www.google.com/search?q=hunde&safe=off&sxsrf=ACYBGNTINmFTZC-rvn4KC2l2Ho4DTt40OA:1577033096830&source=lnms&tbm=isch&sa=X&ved=2ahUKEwjnhqSM2snmAhUPb1AKHTEqAUEQ_AUoAXoECDoQAw&biw=1199&bih=798"]