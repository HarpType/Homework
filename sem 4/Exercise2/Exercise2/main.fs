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


// Функция, делящая список на 2 части: в первой части находятся n элементов,
// во второй - оставшиеся элементы.
let divideList n (list:list<_>) =
    if n <= 0 then
        ([], list)
    else
        let rec iterDivideList n (list:list<_>) (accumList:list<_>) i =
            if n = i then
                (accumList, list)
            else
                match list with
                | h::t -> iterDivideList n t (h::accumList) (i + 1)
                | [] -> (accumList, [])
        iterDivideList n list.Tail [list.Head] 1

// Функция проверяет, является ли заданный список палиндромом.
// Выводит true, если список является палиндромом,
// false в противном случае.
let isPalindrome (list:list<_>) = 
    match divideList (list.Length / 2) list with
    | (list1, list2) ->
        if (list.Length % 2 = 0) then
            listsAreEqual list1 list2
        else
            listsAreEqual list1 list2.Tail
