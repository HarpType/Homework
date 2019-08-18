module LambdaInterpreter

    /// Описывает структуру лямбда-терма
    type LambdaTerm = 
        | Variable of char
        | Application of LambdaTerm * LambdaTerm
        | Abstraction of char * LambdaTerm

    let betaReduction leftTerm rightTerm =
        Variable('t')

    /// Определяет, является ли пара термов редэксом.
    let isRedex leftTerm rightTerm =
        match leftTerm with
        | Abstraction(_) -> true
        | _ -> false

    /// Выполняет бета-редукцию согласно нормальной стратегии
    let rec normalBetaReductionStep term = 
        match term with 
        | Application(leftTerm, rightTerm) -> 
            if isRedex leftTerm rightTerm then
                betaReduction leftTerm rightTerm |> Some
            else
                let leftStep = normalBetaReductionStep leftTerm
                match leftStep with
                | Some(modifiedLeftTerm) -> 
                    Application(modifiedLeftTerm, rightTerm) |> Some
                | None -> 
                    let rightStep = normalBetaReductionStep rightTerm
                    match rightStep with
                    | Some(modifiedRightTerm) -> 
                        Application(leftTerm, modifiedRightTerm) |> Some
                    | None -> None
        | Abstraction(variable, term) ->
            match normalBetaReductionStep term with
            | Some(modifiedTerm) ->
                Abstraction(variable, modifiedTerm) |> Some
            | None -> None
        | Variable(_) -> None 
