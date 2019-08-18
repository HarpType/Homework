module LambdaInterpreter

open System

    /// Описывает структуру лямбда-терма
    type LambdaTerm = 
        | Variable of char
        | Application of LambdaTerm * LambdaTerm
        | Abstraction of char * LambdaTerm

    /// Подставляет в терм leftTerm терм rightTerm вместо всех свободных
    /// вхождений переменной changedVariable согласно правилам.
    let rec substitute leftTerm rightTerm changedVariable =
        match leftTerm with
        | Variable(leftVariable) ->
            if changedVariable = leftVariable then
                rightTerm
            else Variable(leftVariable)
        | Application(leftTerm1, leftTerm2) ->
            let newTerm1 = changedVariable |> (rightTerm |> (leftTerm1 |> substitute))
            let newTerm2 = changedVariable |> (rightTerm |> (leftTerm2 |> substitute))
            Application(newTerm1, newTerm2)
        | Abstraction(leftVariable, leftTerm) ->
            if leftVariable = changedVariable then
                leftTerm
            else
                /// TODO: alpha-conversion
                substitute leftTerm rightTerm changedVariable



    /// Выполняет бета-редукцию на конкретных термах.
    /// Предполагается, что левый терм -- абстракция.
    let betaReduction leftTerm rightTerm =
        match leftTerm with
        | Abstraction(variable, term) ->
            substitute term rightTerm variable
        | _ -> failwith "Left term should be an abstraction."

    /// Определяет, является ли пара термов редэксом.
    let isRedex leftTerm rightTerm =
        match leftTerm with
        | Abstraction(_) -> true
        | _ -> false

    /// Выполняет бета-редукцию согласно нормальной стратегии.
    /// Возвращает Some(term), если бета-редукция прошла успешно,
    /// None в случае, если применение бета-редукции не имеет смысла. 
    let rec normalBetaReductionStep term = 
        match term with 
        | Application(leftTerm, rightTerm) -> 
            if isRedex leftTerm rightTerm then
                betaReduction leftTerm rightTerm |> Some
            else
                match normalBetaReductionStep leftTerm with
                | Some(modifiedLeftTerm) -> 
                    Application(modifiedLeftTerm, rightTerm) |> Some
                | None -> 
                    match normalBetaReductionStep rightTerm with
                    | Some(modifiedRightTerm) -> 
                        Application(leftTerm, modifiedRightTerm) |> Some
                    | None -> None
        | Abstraction(variable, term) ->
            match normalBetaReductionStep term with
            | Some(modifiedTerm) ->
                Abstraction(variable, modifiedTerm) |> Some
            | None -> None
        | Variable(_) -> None 
