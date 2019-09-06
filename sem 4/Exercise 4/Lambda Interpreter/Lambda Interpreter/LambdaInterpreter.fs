module LambdaInterpreter

open System

    /// Описывает структуру лямбда-терма
    type LambdaTerm = 
        | Variable of char
        | Application of LambdaTerm * LambdaTerm
        | Abstraction of char * LambdaTerm

    /// Возвращает список всех свободных переменных в терме.
    let getFreeVariables term =
        /// Рекурсивно обходит терм, аккумулируя множество свободных и связанных переменных.
        let rec recGetFreeVariables tempTerm accumBoundVariables accumFreeVariables = 
            match tempTerm with
            | Variable(variable) ->
                if Set.contains variable accumBoundVariables then
                    accumFreeVariables
                else 
                    Set.add variable accumFreeVariables
            | Application(leftTerm, rightTerm) ->
                let startNextStep term = term |> recGetFreeVariables <| accumBoundVariables <| accumFreeVariables

                Set.union (leftTerm |> startNextStep) (rightTerm |> startNextStep)
            | Abstraction(variable, term) ->
                recGetFreeVariables term (Set.add variable accumBoundVariables) accumFreeVariables

        recGetFreeVariables term Set.empty Set.empty

    /// Возвращает имя новой переменной, которой нет в заданном множестве.
    let getNewVariableName variableNameSet =
        let rec recGetNewVariableName accumVariableName =
            if Set.contains accumVariableName variableNameSet then
                if accumVariableName = 'z' then
                    failwith "Cannot find a new character"
                else 
                    recGetNewVariableName (char ((int accumVariableName) + 1))
            else
                accumVariableName

        recGetNewVariableName 'a'

    /// Производит альфа-конверсию над заданным термом.
    let alphaConversion term oldVariable newVariable = 
        /// Рекурсивно заменяет старую переменную на новую там, где это необходимо.
        let rec recAlphaConversion tempTerm =
            match tempTerm with
            | Variable(variable) ->
                if variable = oldVariable then
                    Variable(newVariable)
                else 
                    tempTerm
            | Application(leftTerm, rightTerm) ->
                Application(leftTerm |> recAlphaConversion, rightTerm |> recAlphaConversion)
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
        | Variable(variable) ->
            if insteadOfVariable = variable then
                whatSubsTerm
            else Variable(variable)
        | Application(leftTerm, rightTerm) ->
            let getNewTerm whereSubsTerm = whereSubsTerm |> substitute <| whatSubsTerm <| insteadOfVariable

            Application(leftTerm |> getNewTerm, rightTerm |> getNewTerm)
        | Abstraction(absVariable, absTerm) ->
            if absVariable = insteadOfVariable then
                whereSubsTerm
            else
                let absTermFreeVarSet = getFreeVariables absTerm
                let rightTermFreeVarSet = getFreeVariables whatSubsTerm

                if (Set.contains absVariable rightTermFreeVarSet) && (Set.contains insteadOfVariable absTermFreeVarSet) then
                    let newVariable = getNewVariableName (Set.union absTermFreeVarSet rightTermFreeVarSet)
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

    /// Выполняет шаг бета-редукции согласно нормальной стратегии.
    /// Возвращает Some(term), если бета-редукция прошла успешно,
    /// None в случае, если применение бета-редукции не имеет смысла. 
    let rec normalBetaReductionStep term = 
        match term with 
        | Variable(_) -> None
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
        | Abstraction(absVariable, absTerm) ->
            match normalBetaReductionStep absTerm with
            | Some(modifiedTerm) ->
                Abstraction(absVariable, modifiedTerm) |> Some
            | None -> None
