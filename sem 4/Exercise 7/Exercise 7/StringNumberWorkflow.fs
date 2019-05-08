module StringNumberWorkflow

    /// Описывает workflow, предназначенный для вычисления чисел, заданные в виде строк.
    type StringCalculatorBuilder() =
        member this.Bind(x:string, f) =
            try 
                int x |> f
            with
            | :? System.FormatException -> None
        member this.Return(x) =
            Some x

    /// Инициализация builder'a.
    let stringCalculator = new StringCalculatorBuilder()
