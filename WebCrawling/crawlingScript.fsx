#I "C:/Users/Jesper/.nuget/packages/fsharp.data/3.3.2/lib/net45/"
#r "FSharp.Data.dll"
open System.Collections.Generic
open System.IO
open System.Text.RegularExpressions

// This library contains F# type providers for working with structured file formats (CSV, HTML, JSON and XML) 
open FSharp.Data

module webcrawler =

    // The "image_scrapper" is used for gathering image urls from the website urls and writing them to a text file
    
    // Signature: Parameter urlLinks is inferred by the compiler as sequence of strings 
    // Returns - absence of a specific value called a unit, represented as such ()
    let image_scraper urlLinks   =
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
            printfn "images: %A " correctImages
            File.AppendAllLines("/Users/edwinsilva/Desktop/WebCrawler/WebCrawling/ImageLinks.txt", correctImages)

            
            

            
    // ExtractRelevantLinks is a function that prepares https links found on the seed to become relevant for the next
    // dive into finding more href tags. 
    // Reason is that href tags have both sub links refering to the website you are on, and links to their own respective site
    // Signature: Parameters: stringList and string which returns string list
    let extractRelevantLinks (list, originpath)  =
        let pattern1 = "(?i)^https://|^http://|^#|^j"
        let pattern2 = "(?i)^https://|^http://"
        let subLinks = list |> List.filter (fun x -> Regex(pattern1).IsMatch(x) = false) |> List.map(fun x -> originpath + x)
        let mainLinks = list |> List.filter (fun x -> Regex(pattern2).IsMatch(x))
        mainLinks @ subLinks

    // mutable list visited is used to keep track of the already visited sites.
    // reason being that you might end up in a loop if you allow for visiting the same website again
    let mutable visited = new List<string>()
    
    // Follow_links is a recursive function that runs through multiple sites getting the hrefs.values from their html, 
    // If the list becomes empty, the function has run out of urls to crawl, 
    // and simply prints the count of visited sites. 
    // Signature: Parameters - list of string and returns a unit/()
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
            image_scraper canBeCrawled
            follow_links canBeCrawled
        else 
            follow_links xs



       


   

    // Initial seed
    follow_links ["https://www.google.com/search?q=hunde&safe=off&sxsrf=ACYBGNTINmFTZC-rvn4KC2l2Ho4DTt40OA:1577033096830&source=lnms&tbm=isch&sa=X&ved=2ahUKEwjnhqSM2snmAhUPb1AKHTEqAUEQ_AUoAXoECDoQAw&biw=1199&bih=798"]