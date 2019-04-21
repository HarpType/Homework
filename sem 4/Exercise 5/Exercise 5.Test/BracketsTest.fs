namespace Tests

module BracketsTest = 
    open NUnit.Framework
    open FsUnit
    open Brackets 

    let standartIsBracketsStringCorrect = bracketsStringChekerFactory ('(', ')') ('[', ']') ('{', '}')
    let ``isBracketsStringCorrectOnlyWith()`` = bracketsStringChekerFactory ('(', ')') ('(', ')') ('(', ')')
    let ``isBracketsStringCorrectOnlyWith(){}`` = bracketsStringChekerFactory ('(', ')') ('{', '}') ('(', ')')
    let ``isBracketsStringCorrectThreeOpeningOneClosing`` = bracketsStringChekerFactory ('(', ')') ('{', ')') ('[', ')')

    [<Test>]
    let ``Correct brackets string with [], (), {}`` ()=        
        "[bl{abla(blab{}()la)}]" |> standartIsBracketsStringCorrect |> should equal true

    [<Test>]
    let ``Incorrect brackets string with [], (), {}`` ()=
        "[hello{)()]]" |> standartIsBracketsStringCorrect |> should equal false

    [<Test>]
    let ``Correct brackets string only with ()`` ()=
        "{{{{ (((o(*°▽°*)o))) ╰(▔∀▔)╯  (ﾉ´ヮ)ﾉ*: ･ﾟ" |> ``isBracketsStringCorrectOnlyWith()`` |> should equal true

    [<Test>]
    let ``Incorrect brackets string only with (), {}`` ()=
        "({}((}" |> ``isBracketsStringCorrectOnlyWith(){}`` |> should equal false

    [<Test>]
    let ``Correct brackets string with three opening and one closing`` ()=
        "() [) {) ({[))) [])" |> isBracketsStringCorrectThreeOpeningOneClosing |> should equal true
    
    [<Test>]
    let ``InCorrect brackets string with three opening and one closing`` ()=
        "() {} []" |> isBracketsStringCorrectThreeOpeningOneClosing |> should equal false

    [<Test>]
    let failWithTest ()=
        (fun () -> bracketsStringChekerFactory ('(', '(') ('[', ']') ('{', '}') |> ignore) |> should throw typeof<System.Exception>



