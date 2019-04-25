module PhoneBookTest
    open NUnit.Framework
    open FsUnit
    open PhoneBook

    let testPhoneBook = 
        let add1 = {Name="Ivan Ivanov"; PhoneNumber="89349256887"} |> addPhoneRecord EmptyNode
        let add2 = {Name="Alex Al"; PhoneNumber="86349539287"} |> addPhoneRecord add1
        let add3 = {Name="Kir Korov"; PhoneNumber="89157785688"} |> addPhoneRecord add2
        let add4 = {Name="Pavel Pavel"; PhoneNumber="22345359686"} |> addPhoneRecord add3
        let add5 = {Name="Ulia Ul"; PhoneNumber="56899829846"} |> addPhoneRecord add4
        let add6 = {Name="Kir Korov"; PhoneNumber="89399893846"} |> addPhoneRecord add5
        let add7 = {Name="Pul Lya"; PhoneNumber="79832829642"} |> addPhoneRecord add6

        add7

    [<Test>]
    let ``89833435326 is a correct phone number`` ()=
        "89833435326" |> isPhoneNumber |> should equal true
        "+79833435326" |> isPhoneNumber |> should equal false

    [<Test>]
    let ``8kfs is not a correct phone number`` ()=
        "8kfs" |> isPhoneNumber |> should equal false

    [<Test>]
    let uniquePhoneTest ()=
        let newAdd = {Name="Igorrr"; PhoneNumber="89399893846"} |> addPhoneRecord testPhoneBook

        findByPhoneNumber newAdd "89399893846" |> Option.get |> should equal "Igorrr"

    [<Test>]
    let findByPhonenumberTest ()=
        findByPhoneNumber testPhoneBook "86349539287" |> Option.get |> should equal "Alex Al"
        findByPhoneNumber testPhoneBook "22345359686" |> Option.get |> should equal "Pavel Pavel"
        findByPhoneNumber testPhoneBook "89239348658" |> should equal None


    [<Test>]
    let findByNameTest ()=
        savePhoneBookToFile testPhoneBook "test.pb"

        let phones = findByName testPhoneBook "Kir Korov"

        Seq.contains "89399893846" phones |> should equal true
        Seq.contains "89157785688" phones |> should equal true


