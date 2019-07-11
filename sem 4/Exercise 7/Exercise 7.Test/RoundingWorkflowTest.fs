namespace Tests

module RoundingWorkflowTest =

    open NUnit.Framework
    open FsUnit
    open RoundingWorkflow

    [<Test>]
    let example1Test () =
        let example = rounding 3 {
                let! a = 2.0 / 12.0
                let! b = 3.5
                return a / b
            }
        example |> should (equalWithin 0.001) 0.048

    [<Test>]
    let example2Test () =
        let example = rounding 5 {
                let! a = 3.0 / 0.
                return a
            }
        example |> should equal infinity

    [<Test>]
    let complicatedTest () =
        let example = rounding 4 {
                let! a = 38424.32413241
                let! b = 8993472.324
                let! c = 5.
                let! pi = 3.1415926535897932
                return c / a * b - pi
            }
        example |> should (equalWithin 0.0001) 1167.1421
