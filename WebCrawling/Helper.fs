namespace Console
open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.IO
open System.Net
open System.Text.RegularExpressions

module Helpers =

    type Message =
        | Done
        | Mailbox of MailboxProcessor<Message>
        | Stop
        | Url of string option
        | Start of AsyncReplyChannel<unit>

    // Gates the number of crawling agents.
    [<Literal>]
    let Gate = 5

    // Extracts links from HTML.
    let extractLinks html =
        let pattern1 = "(?i)href\\s*=\\s*(\"|\')/?((?!#.*|/\B|" + 
                       "mailto:|location\.|javascript:)[^\"\']+)(\"|\')"
        let pattern2 = "(?i)^https?"
 
        let links =
            [
                for x in Regex(pattern1).Matches(html) do
                    yield x.Groups.[2].Value
            ]
            |> List.filter (fun x -> Regex(pattern2).IsMatch(x))
        links
    
    // Fetches a Web page.
    let fetch (url : string) =
        try
            let req = WebRequest.Create(url) :?> HttpWebRequest
            req.UserAgent <- "Mozilla/5.0 (Windows; U; MSIE 9.0; Windows NT 9.0; en-US)"
            req.Timeout <- 5000
            use resp = req.GetResponse()
            let content = resp.ContentType
            let isHtml = Regex("html").IsMatch(content)
            match isHtml with
            | true -> use stream = resp.GetResponseStream()
                      use reader = new StreamReader(stream)
                      let html = reader.ReadToEnd()
                      Some html
            | false -> None
        with
        | _ -> None
    
    let collectLinks url =
        let html = fetch url
        match html with
        | Some x -> extractLinks x
        | None -> []
