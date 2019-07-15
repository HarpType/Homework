namespace Tests

module SitesSizeTest = 
    open NUnit.Framework
    open FsUnit
    open SitesSize

    [<Test>]
    let ``Google test`` () = 
        let sitesInfo = "http://www.google.com" |> getSitesInfo |> Option.get
        sitesInfo.Length |> should equal 1
        sitesInfo.[0] |> should equal "http://www.google.ru/intl/ru/services/ --- 75531"

    [<Test>]
    let ``NonExistentURL test`` () =
        let optionSitesInfo = "NonExistentURL.com" |> getSitesInfo
        optionSitesInfo |> should equal None
