module Rounding 
    open System
    
    type RoundingBuilder(round:int) =
        member this.Bind(x, f) =
            f x
        member this.Return(x:float) =
            System.Math.Round(x, round)

    let rounding round = new RoundingBuilder(round)

    let answer = rounding 3 {
        let! a = 2.0 / 12.0
        let! b = 3.5
        return a / b
    }

