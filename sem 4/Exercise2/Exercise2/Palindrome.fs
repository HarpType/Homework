module Palindrome

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
