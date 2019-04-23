namespace Tests

module FirstPosition =
    open NUnit.Framework
    open FsUnit
    open Main

    [<Test>]
    let ``First position of 3 is 3`` ()=
        firstPosInList 3 [0; 1; 2; 3; 4; 5] |> Option.get |> should equal 3

    [<Test>]
    let ``First position of 0 is 0`` ()=
        firstPosInList 0 [0; 1; 0; 1] |> Option.get |> should equal 0

    [<Test>]
    let ``2 is not on a list`` ()=
        firstPosInList 2 [0; 1; 0; 1] |> should equal None

    [<Test>]
    let ``0 is not on an empty list`` ()=
        firstPosInList 0 [] |> should equal None
