namespace Test

module QueueTest =
    
    open NUnit.Framework
    open FsUnit
    open PriorQueue

    [<Test>]
    let priorQueueTest ()=
        let priorQueue = PriorQueue([])

        priorQueue.insert 2 "p"
        priorQueue.insert 1 "k"
        priorQueue.findMin |> should equal {Priority=1; Item="k"}
