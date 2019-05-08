module RoundingWorkflow 

    open System
    
    /// Тип-builder. Описывает workflow, предназначенный для математических вычислений с заданной точностью.
    /// Заданная точность задаётся как аргумент конструктора типа.
    type RoundingBuilder(round:int) =
        member this.Bind(x, f) =
            f x
        member this.Return(x:float) =
            System.Math.Round(x, round)

    /// Инициализация builder'а.
    let rounding round = new RoundingBuilder(round)
