namespace Tests

module LambdaInterpreter =
    open NUnit.Framework
    open FsUnit
    open LambdaInterpreter

    [<Test>]
    let simpleTest () =
        /// (λx.x y) z -> z y
        let simpleLambdaTerm = Application(Abstraction('x', Application(Variable('x'), Variable('y'))), Variable('z'))
        
        simpleLambdaTerm |> normalBetaReductionStep |> Option.get 
                |> should equal (Application(Variable('z'), Variable('y')))


    