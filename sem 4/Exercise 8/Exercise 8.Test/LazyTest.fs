module LazyTest

    open NUnit.Framework
    open FsUnit

    open Lazy.LazyFactory
    open Lazy.ILazy

    let rnd = System.Random()

    //--------Тестирование однопоточного ленивого вычисления--------

    [<Test>]
    let squareListTest () =
        let correctSquareList = List.init 5 (fun index -> index * index)

        let computeTestSquareList () = 
            let list = [0; 1; 2; 3; 4]
            List.map (fun item -> item * item) list

        let squareListLazy = LazyFactory.CreateSingleThreadedLazy(computeTestSquareList)

        (squareListLazy :> ILazy<int list>).Get() |> should equal correctSquareList

    [<Test>]
    let randomTest () =
        let randomLazy = LazyFactory.CreateSingleThreadedLazy(fun () -> rnd.Next(0, 99))

        let firstAnswer = (randomLazy :> ILazy<int>).Get()

        for _ in 0..9 do
            (randomLazy :> ILazy<int>).Get() |> should equal firstAnswer



    
