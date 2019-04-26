module SequencesTest

    open NUnit.Framework
    open FsUnit
    open Sequences

    [<Test>]
    let plusMinusOneSequenceTest ()=
        plusMinusOneSequence |> Seq.take 5 |> Seq.toList |> should equal [1; -1; 1; -1; 1]

    [<Test>]
    let superPlusMinusSequenceTest ()=
        superPlusMinusSequence |> Seq.take 5 |> Seq.toList |> should equal [1; -2; 3; -4; 5]

