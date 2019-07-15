module SitesSize

    open System
    open System.Net
    open System.IO
    open System.Text.RegularExpressions

    // Скачивает страницу сайта по url. В случае удачи оборачивает информацию о сайте в Some.
    // В противном случае возвращает None.
    let getSiteData url = 
        try
            let request = WebRequest.Create(Uri(url))
            use response = request.GetResponse()
            use stream = response.GetResponseStream()
            use reader = new StreamReader(stream)
            let html = reader.ReadToEnd()

            Some(html)
        with
        | _ -> None 

    // Узнаёт размер сайта.
    let fetchSiteSizeAsync url = 
        async {
            let html = getSiteData url
            match html with
            | Some(data) -> 
                return url + " --- " + data.Length.ToString() |> Some
            | None -> 
                return None
        }

    let expression = @"<a\s*href=""(http:\/\/\S+)"".*>"

    // Скачивает все веб-страницы, на которые указывает сайт и предоставляет 
    // информацию о их размерах (в массиве).
    let printSitesInfo url =
        let html = getSiteData url
        match html with
        | Some(data) -> 
            let regex = new Regex(expression)
            let mutable m = regex.Match data
            let siteSeq = [while (m.Success) do yield m.Groups.[1].ToString(); m <- m.NextMatch()]

            siteSeq |> Seq.map (fun site -> site |> fetchSiteSizeAsync) 
                    |> Async.Parallel |> Async.RunSynchronously 
                    |> Seq.filter(fun optionSiteInfo -> optionSiteInfo.IsSome)
                    |> Seq.map(fun optionSiteInfo -> optionSiteInfo.Value)
                    |> Some
        | None -> None
            
        

