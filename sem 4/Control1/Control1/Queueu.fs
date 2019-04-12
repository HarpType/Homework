module Queueu

type Queue(fifo : list<_>) =
    let mutable fifoList = fifo
    member q.push a = a :: fifo
    member q.pop = 
        


