module StringNumberWorkflow

    /// Описывает workflow, предназначенный для вычисления чисел, заданные в виде строк.
    type StringCalculatorBuilder() =
        member this.Bind(x:string, f) =
            match System.Int32.TryParse(x) with
            | (true, int) -> x |> f
            | _ -> None
        member this.Return(x) =
            Some x

    /// Инициализация builder'a.
    let stringCalculator = new StringCalculatorBuilder()
