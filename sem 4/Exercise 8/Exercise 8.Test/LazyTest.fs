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

    //--------Тестирование многопоточного ленивого вычисления--------
    [<Test>]
    let safeMultipleThreadLazyCorrectTest () =
        let correctSupplier = fun () -> "I'm correct"
        let correctSafeLazy = LazyFactory.CreateSafeMultipleThreadLazy(correctSupplier)

        let mutable resultArray = Array.zeroCreate 5

        let fetchCorrectAsync threadNumber = 
            async {
                resultArray.[threadNumber] <- (correctSafeLazy :> ILazy<string>).Get()
            }

        let threadNumbers = List.init 5 (fun index -> index)
        do threadNumbers |> List.map (fun threadNumber -> threadNumber |> fetchCorrectAsync)
                        |> Async.Parallel |> Async.RunSynchronously |> ignore

        do resultArray |> Array.map (fun result -> result |> should equal "I'm correct") |> ignore

    [<Test>]
    let safeMultipleThreadLazyRandomTest () =
        let randomSupplier = fun () -> rnd.Next(0, 99)
        let randomSafeLazy = LazyFactory.CreateSafeMultipleThreadLazy(randomSupplier)

        let mutable resultArray = Array.init 10 (fun _ -> Array.zeroCreate 10)

        let fetchRandomAsync threadNumber = 
            async {
                for i in 0..9 do 
                    resultArray.[threadNumber].[i] <- (randomSafeLazy :> ILazy<int>).Get()
            }

        let threadNumbers = List.init 10 (fun index -> index)

        do threadNumbers |> List.map (fun threadNumber -> threadNumber |> fetchRandomAsync)
                        |> Async.Parallel |> Async.RunSynchronously |> ignore

        let controlResult = resultArray.[0].[0]

        for i in 0..9 do
            for j in 0..9 do
                resultArray.[i].[j] |> should equal controlResult

    //--------Тестирование lock-free многопоточного ленивого вычисления--------
    [<Test>]
    let LockFreeMultipleThreadLazyCorrectTest () =
        let correctSupplier = fun () -> "I'm correct"
        let correctLockFreeLazy = LazyFactory.CreateLockFreeMultipleThreadLazy(correctSupplier)

        let mutable resultArray = Array.zeroCreate 5

        let fetchCorrectAsync threadNumber = 
            async {
                resultArray.[threadNumber] <- (correctLockFreeLazy :> ILazy<string>).Get()
            }

        let threadNumbers = List.init 5 (fun index -> index)
        do threadNumbers |> List.map (fun threadNumber -> threadNumber |> fetchCorrectAsync)
                        |> Async.Parallel |> Async.RunSynchronously |> ignore

        do resultArray |> Array.map (fun result -> result |> should equal "I'm correct") |> ignore

    [<Test>]
        let LockFreeMultipleThreadLazyRandomTest () =
            let randomSupplier = fun () -> rnd.Next(0, 99)
            let randomLockFreeLazy = LazyFactory.CreateLockFreeMultipleThreadLazy(randomSupplier)

            let mutable resultArray = Array.init 10 (fun _ -> Array.zeroCreate 10)

            let fetchRandomAsync threadNumber = 
                async {
                    for i in 0..9 do 
                        resultArray.[threadNumber].[i] <- (randomLockFreeLazy :> ILazy<int>).Get()
                }

            let threadNumbers = List.init 10 (fun index -> index)

            do threadNumbers |> List.map (fun threadNumber -> threadNumber |> fetchRandomAsync)
                            |> Async.Parallel |> Async.RunSynchronously |> ignore

            let controlResult = resultArray.[0].[0]

            for i in 0..9 do
                for j in 0..9 do
                    resultArray.[i].[j] |> should equal controlResult
