module PriorQueue

    type QueueItem = {Priority:int; Item:string}

    type PriorQueue(queList:list<QueueItem>) =
        let mutable queueList = queList
        member q.insert priority item = queueList <- {Priority=priority; Item=item}::queueList
        member q.findMin = queueList |> List.minBy (fun item -> item.Priority)

