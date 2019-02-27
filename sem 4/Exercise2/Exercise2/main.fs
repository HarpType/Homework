(* Блок задачи 1: вхождение заданного числа в список*)

// Функция, возвращающая позицию первого заданного элемента в заданном списке.
// Если заданного элемента в списке нет, возвращает None.
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


(* Блок задачи 2: проверка на палиндром*)

// Функция проверяет 2 списка на равенство. Выводит true, если списки равны,
// false в противном случае.
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