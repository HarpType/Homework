let firstPosInList x list = 
    let rec iterFirstPosInList x list i =
        match list with 
        | [] -> None
        | h::t -> 
            if x = h then
                Some(i)
            else
                iterFirstPosInList x list.Tail (i + 1)
    iterFirstPosInList x list 1


(* Блок с палиндромом*)
let rec listsAreEqual (list1:list<_>) (list2:list<_>) =
    match (list1,list2) with
    | (h1::t1, h2::t2) -> 
        if h1 = h2 then
            listsAreEqual t1 t2
        else
            false
    | ([],h::t) | (h::t,[]) -> false
    | ([],[]) -> true

let answer = listsAreEqual [1; 2; 3] [1; 2; 3]