namespace Tests

module LambdaInterpreter =
    open NUnit.Framework
    open FsUnit
    open LambdaInterpreter

    [<Test>]
    let simpleTest () =
        /// (λx.x y) z -> z y
        let simpleLambdaTerm = Application( 
                                Abstraction('x', Application(Variable('x'), Variable('y'))), 
                                Variable('z'))
        
        simpleLambdaTerm |> normalBetaReductionStep |> Option.get 
                |> should equal (Application(Variable('z'), Variable('y')))

    [<Test>]
    let alphaConversionTest () =
        /// (λx.λy.x y) (λx.x y) -> λa.(λx.x y) a   (с альфа конверсией)
        let termForAlphaConversion = Application(
                                        Abstraction('x', Abstraction('y', Application(Variable('x'), Variable('y')))), 
                                        Abstraction('x', Application(Variable('x'), Variable('y'))))

        termForAlphaConversion |> normalBetaReductionStep |> Option.get 
                |> should equal (Abstraction('a', Application(Abstraction('x', Application(Variable('x'), 
                                                                                            Variable('y'))), 
                                                                Variable('a'))))

    [<Test>]
    let ``S K K = I`` () =
        let SKKTerm = Application(
                        Application(
                            Abstraction('x', 
                                Abstraction('y', 
                                    Abstraction('z', Application(Application(Variable('x'), Variable('z')), 
                                                                    Application(Variable('y'), Variable('z')))))),
                            Abstraction('x', Abstraction('y', Variable('x')))),
                        Abstraction('x', Abstraction('y', Variable('x'))))
        
        let mutable checkTerm = SKKTerm
        for _ in 1 .. 4 do
            checkTerm <- checkTerm |> normalBetaReductionStep |> Option.get

        checkTerm |> should equal (Abstraction('z', Variable('z')))

        /// Нормальная форма уже достигнута, следующий шаг возвращает None.
        checkTerm |> normalBetaReductionStep |> should equal None

    [<Test>]
    let ``K I = K*`` () =
        let KITerm = Application(
                        Abstraction('x', Abstraction('y', Variable('x'))),
                        Abstraction('x', Variable('x')))

        KITerm |> normalBetaReductionStep |> Option.get 
                |> should equal (Abstraction('y', Abstraction('x', Variable('x'))))
