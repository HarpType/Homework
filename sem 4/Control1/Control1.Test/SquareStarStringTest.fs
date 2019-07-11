module SquareStarStringTest

module SumFibonacciTest =
    open NUnit.Framework
    open FsUnit
    open StarsPrint

    let string1x1 = "*\n"
    let string2x2 = "**\n**\n"
    let string4x4 = "****\n*  *\n*  *\n****\n"

    [<Test>]
    let ``Square stars string 1x1`` ()=
        squareStarString 1 |> should equal string1x1

    [<Test>]
    let ``Square stars string 2x2`` ()=
        squareStarString 2 |> should equal string2x2

    [<Test>]
    let ``Square stars string 4x4`` ()=
        squareStarString 4 |> should equal string4x4

    [<Test>]
    let ``Square stars string -5`` ()=
        squareStarString -5 |> should equal ""

