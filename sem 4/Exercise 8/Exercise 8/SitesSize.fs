module SitesSize

    open System
    open System.Net
    open System.IO
    open System.Text.RegularExpressions

    // Скачивает страницу сайта по url
    let getSiteData url = 
        let request = WebRequest.Create(Uri(url))
        use response = request.GetResponse()
        use stream = response.GetResponseStream()
        use reader = new StreamReader(stream)
        let html = reader.ReadToEnd()

        html

    // Узнаёт размер сайта
    let fetchSiteSizeAsync url = 
        async {
            let html = getSiteData url
            let answer =  url + "---" + html.Length.ToString()
            return answer
        }

    let expression = @"<a href=""(http:\/\/\S+)"">"

    // Скачивает все веб-страницы, на которые указывает сайт и предоставляет 
    // информацию о их размерах.
    let printSitesInfo url =
        // let html = getSiteData url
        let regex = new Regex(expression)
        let mutable m = regex.Match url
        let siteSeq = [while (m.Success) do yield m.Groups.[1].ToString(); m <- m.NextMatch()]

        siteSeq |> Seq.map (fun site -> site |> fetchSiteSizeAsync) 
                |> Async.Parallel |> Async.RunSynchronously
            
        

