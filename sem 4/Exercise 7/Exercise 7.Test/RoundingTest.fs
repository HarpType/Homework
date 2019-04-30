namespace Tests

module RoundingTest =

    open NUnit.Framework
    open FsUnit
    open Rounding

    let example1 = rounding 3 {
        let! a = 2.0 / 12.0
        let! b = 3.5
        return a / b
    }

    let example2 = rounding 5 {
        let! a = 3.0 / 0.
        return a
    }

    [<Test>]
    let example1Test ()=
        example1 |> should equal 0.048

    [<Test>]
    let example2Test ()=
        example2 |> should equal infinity

