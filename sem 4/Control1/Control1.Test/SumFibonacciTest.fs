namespace Tests

module SumFibonacciTest =
    open NUnit.Framework
    open FsUnit
    open SumFibonacci

    [<Test>]
    let ``Negative input data`` ()=
        sumFibonacci -5 |> should equal 0

    [<Test>]
    let ``Sum of even fibonacci numbers with sill of 100`` ()=
        sumFibonacci 100 |>should equal 44

    [<Test>]
    let ``Sum of even fibonacci numbers with sill of 200`` ()=
        sumFibonacci 200 |> should equal 188

    [<Test>]
    let ``Sum of even fibonacci numbers with sill of 600`` ()=
        sumFibonacci 600 |> should equal 188

    [<Test>]
    let ``Sum of even fibonacci numbers with sill of 1000000`` ()=
        sumFibonacci 1000000 |> should equal 1089154

