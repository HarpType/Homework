namespace Tests

module Palindrome =
    open NUnit.Framework
    open FsUnit
    open Main

    [<Test>]
    let ``Empty sting is a palindrome`` ()=
        isPalindrome "" |> should be True

    [<Test>]
    let ``One character string is a palindrome`` ()=
        isPalindrome "O" |> should be True

    [<Test>]
    let ``"May a moody baby doom a yam" is a palindrome`` ()=
        isPalindrome "May a moody baby doom a yam" |> should be True

    [<Test>]
    let ``"London is the capital of Great Britain" is not a palindrome`` ()=
        isPalindrome "London is the capital of Great Britain" |> should not' (be True)

