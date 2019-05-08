namespace Tests

module StringNumberWorkflowTest =

    open NUnit.Framework
    open FsUnit
    open StringNumberWorkflow

    [<Test>]
    let correctCalculate ()=
        let result = stringCalculator {
                let! x = "1"
                let! y = "2"
                let z = x + y
                return z
            }
        result |> Option.get |> should equal 3
        
    [<Test>]
    let wrongCalculate ()=
        let result = stringCalculator {
                let! x = "1"
                let! y = "Ъ"
                let z = x + y
                return z
            }
        result |> should equal None

    [<Test>]
    let worksWithInt ()=
        let result = stringCalculator {
                let! x = "1."
                let! y = "2"
                let z = x + y
                return z
            }
        result |> should equal None

    [<Test>]
    let exceptionTest ()=
        let result () = stringCalculator {
                let! x = "1"
                let! y = "0"
                let z = x / y
                return z
            }
        (fun () -> result () |> ignore) |> should throw typeof<System.DivideByZeroException>

    [<Test>]
    let complicatedTest ()=
        let result = stringCalculator {
                let! x = "5790"
                let! y = "89"
                let z = x / y
                let! u = "8"
                let zz = u * z
                let! v = "10"
                let! l = "732"
                return zz / v + l
            }
        result |> Option.get |> should equal 784



