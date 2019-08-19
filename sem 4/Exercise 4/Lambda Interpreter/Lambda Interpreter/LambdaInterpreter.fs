module LambdaInterpreter

    /// Описывает структуру лямбда-терма
    type LambdaTerm = 
        | Variable of char
        | Application of LambdaTerm * LambdaTerm
        | Abstraction of char * LambdaTerm

    /// Возвращает список всех свободных переменных в терме.
    let getFreeVariables term =
        let rec recGetFreeVariables tempTerm accumBoundVariables accumFreeVariables = 
            match tempTerm with
            | Variable(variable) ->
                if Set.contains variable accumBoundVariables then
                    accumFreeVariables
                else 
                    Set.add variable accumFreeVariables
            | Application(leftTerm, rightTerm) ->
                /// WARNING: awful code here
                Set.union (recGetFreeVariables leftTerm accumBoundVariables accumFreeVariables) 
                            (recGetFreeVariables rightTerm accumBoundVariables accumFreeVariables)
            | Abstraction(variable, term) ->
                recGetFreeVariables term (Set.add variable accumBoundVariables) accumFreeVariables

        recGetFreeVariables term Set.empty Set.empty

    /// Возвращает новый символ, которого нет в заданном множестве.
    let getNewCharacter charSet =
        let rec recGetNewCharacter charSet accumChar =
            if Set.contains accumChar charSet then
                if accumChar = 'z' then
                    failwith "Cannot find a new character"
                else 
                    recGetNewCharacter charSet (char ((int accumChar) + 1))
            else
                accumChar

        recGetNewCharacter charSet 'a'

    /// Производит альфа-конверсию над заданным термом.
    let alphaConversion term oldVariable newVariable = 
        let rec recAlphaConversion tempTerm =
            match tempTerm with
            | Variable(variable) ->
                if variable = oldVariable then
                    Variable(newVariable)
                else 
                    tempTerm
            | Application(leftTerm, rightTerm) ->
                Application(recAlphaConversion leftTerm, recAlphaConversion rightTerm)
            | Abstraction(absVariable, absTerm) ->
                if absVariable = oldVariable then
                    Abstraction(newVariable, recAlphaConversion absTerm)
                else
                    Abstraction(absVariable, recAlphaConversion absTerm)

        recAlphaConversion term

    /// Подставляет в терм leftTerm терм rightTerm вместо всех свободных
    /// вхождений переменной changedVariable согласно правилам.
    /// whereSubsTerm -- в какой терм вставлять.
    /// whatSubsTerm -- что за терм вставлять.
    /// insteadOfVariable -- взамен какой переменной вставлять терм.
    let rec substitute whereSubsTerm whatSubsTerm insteadOfVariable =
        match whereSubsTerm with
        | Variable(leftVariable) ->
            if insteadOfVariable = leftVariable then
                whatSubsTerm
            else Variable(leftVariable)
        | Application(leftApplTerm, rightApplTerm) ->
            /// Warning: awful code here
            let newTerm1 = insteadOfVariable |> (whatSubsTerm |> (leftApplTerm |> substitute))
            let newTerm2 = insteadOfVariable |> (whatSubsTerm |> (rightApplTerm |> substitute))
            Application(newTerm1, newTerm2)
        | Abstraction(absVariable, absTerm) ->
            if absVariable = insteadOfVariable then
                whereSubsTerm
            else
                let absTermFreeVarSet = getFreeVariables absTerm
                let rightTermFreeVarSet = getFreeVariables whatSubsTerm

                if (Set.contains absVariable rightTermFreeVarSet) && (Set.contains insteadOfVariable absTermFreeVarSet) then
                    let newVariable = getNewCharacter (Set.union absTermFreeVarSet rightTermFreeVarSet)
                    Abstraction(newVariable, insteadOfVariable |> (whatSubsTerm |> (alphaConversion absTerm absVariable newVariable |> substitute)))
                else
                    Abstraction(absVariable, substitute absTerm whatSubsTerm insteadOfVariable)


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
