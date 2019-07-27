module SitesSizeTest 

    open NUnit.Framework
    open FsUnit
    open SitesSize
    open System.Text.RegularExpressions

    [<Test>]
    let ``NonExistentURL test`` () =
        let optionSitesInfo = "NonExistentURL.com" |> getSitesInfo
        optionSitesInfo |> should equal None

    [<Test>]
    let ``Expression test`` () =
        let testString = """
            <!doctype html>
            <html>
            <head>
                <title>Example Domain</title> 
            </head>

            <body>
            <div>
                <h1>Example Domain</h1>
                <p><a href="http://www.example1.org">Information...</a></p>
                <p><a href="http://www.example2.org/example" id=1></a><p>
                <p><a     href="http://www.example3.org">Hello text</a><p>

            </div>
            </body>
            </html>
        """

        let regex = new Regex(expression)
        let mutable m = regex.Match testString
        let siteSeq = 
            [
                while (m.Success) do 
                    yield m.Groups.[1].ToString()
                    m <- m.NextMatch()
            ]

        siteSeq.Length |> should equal 3
        siteSeq.[0] |> should equal "http://www.example1.org"
        siteSeq.[1] |> should equal "http://www.example2.org/example"
        siteSeq.[2] |> should equal "http://www.example3.org"