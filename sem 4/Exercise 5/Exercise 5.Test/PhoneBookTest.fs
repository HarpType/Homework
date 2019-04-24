module PhoneBookTest
    open NUnit.Framework
    open FsUnit
    open PhoneBook

    [<Test>]
    let ``89833435326 is a correct phone number`` ()=
        "89833435326" |> isPhoneNumber |> should equal true
        "+79833435326" |> isPhoneNumber |> should equal false

    [<Test>]
    let ``8kfs is not a correct phone number`` ()=
        "8kfs" |> isPhoneNumber |> should equal false

    [<Test>]
    let addFindToRecordTreeCkeck ()=
        let add1 = {Name="Ivan Ivanov"; PhoneNumber="89349256887"} |> addPhoneRecord EmptyNode
        let add2 = {Name="Alex Al"; PhoneNumber="86349539287"} |> addPhoneRecord add1
        let add3 = {Name="Kir Korov"; PhoneNumber="891577856887"} |> addPhoneRecord add2
        let add4 = {Name="Pavel Pavel"; PhoneNumber="22345359686"} |> addPhoneRecord add3
        let add5 = {Name="Ulia Ul"; PhoneNumber="56899829846"} |> addPhoneRecord add4

        findByPhoneNumber add5 "86349539287" |> Option.get |> should equal "Alex Al"
        findByPhoneNumber add5 "22345359686" |> Option.get |> should equal "Pavel Pavel"
        findByPhoneNumber add5 "89239348658" |> should equal None

