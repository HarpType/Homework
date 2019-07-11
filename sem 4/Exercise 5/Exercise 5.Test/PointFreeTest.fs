namespace Test

module PointFreeTest =
    open NUnit.Framework
    open FsCheck
    open PointFree

    let funcIsfunc1 x l =
        func x l = func1 x l

    let funcIsfunc2 x l =
        func x l = func2 x l

    let funcIsfunc3 x l = 
        func x l = func3 x l

    [<Test>]
    let checkfuncIsfunc1 ()=
        Check.QuickThrowOnFailure funcIsfunc1

    [<Test>]
    let checkfuncIsfunc2 ()=
        Check.QuickThrowOnFailure funcIsfunc2

    [<Test>]
    let checkfuncIsfunc3 ()=
        Check.QuickThrowOnFailure funcIsfunc3

