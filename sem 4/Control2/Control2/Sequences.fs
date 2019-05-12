module Sequences
    
    /// Бесконечная последовательность вида [1, -1, 1, -1, 1, ...]
    let plusMinusOneSequence = Seq.initInfinite (fun index -> if index % 2 = 0 then 1 else -1)

    /// Бесконечная последовательнос вида [1, -2, 3, -4, 5, -6, ...], используащая последовательнос вида [1, -1, 1, -1, ...]
    let superPlusMinusSequence = 
        let rec moveSeq accum (sequence:seq<int>) =
            let newItem = accum * -1 + (sequence |> Seq.head)
            seq {yield newItem; yield! moveSeq newItem (sequence |> Seq.tail)}

        moveSeq 0 plusMinusOneSequence

    ///let seqPrinter = plusMinusOneSequence |> Seq.take 10 |> Seq.iter (fun item -> printf "%d " item)
