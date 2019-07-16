namespace Tests

module SitesSizeTest = 
    open NUnit.Framework
    open FsUnit
    open SitesSize

    [<Test>]
    let ``NonExistentURL test`` () =
        let optionSitesInfo = "NonExistentURL.com" |> getSitesInfo
        optionSitesInfo |> should equal None
