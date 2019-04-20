(* Блок задачи 1: вхождение заданного числа в список*)

/// Функция, возвращающая позицию первого заданного элемента в заданном списке.
/// Если заданного элемента в списке нет, возвращает None.
let firstPosInList x list = 
    let rec iterFirstPosInList x list i =
        match list with 
        | [] -> None
        | h::t -> 
            if x = h then
                Some(i)
            else
                iterFirstPosInList x list.Tail (i + 1)
    iterFirstPosInList x list 0


(* Блок задачи 2: проверка на палиндром*)

/// Функция, делящая список на 2 части: в первой части находятся отображённые n элементов,
/// во второй - оставшиеся элементы.
let reverseElements n (list:list<_>) =
    if n <= 0 then
        ([], list)
    else
        let rec iterDivideList n (list:list<_>) (accumList:list<_>) i =
            if n = i then
                (accumList, list)
            else
                match list with
                | h::tail -> iterDivideList n tail (h::accumList) (i + 1)
                | [] -> (accumList, [])
        iterDivideList n list.Tail [list.Head] 1

/// Функция проверяет, является ли заданный список палиндромом.
/// Выводит true, если список является палиндромом,
/// false в противном случае.
let isListPalindrome (list:list<_>) = 
    let listLength = list.Length

    match reverseElements (listLength / 2) list with
    | (list1, list2) ->
        if (listLength % 2 = 0) then
            list1 = list2
        else
            list1 = list2.Tail

/// Проверяет, является ли заданная строка палиндромом.
let isPalindrome (str:string) =
     str.ToLower().ToCharArray() |> Array.toList |> 
        List.filter(fun c -> c <> ' ') |> isListPalindrome


(* Блока задачи 3: сортировка слиянием*)

/// Функция делит заданный список list на два, помещая одну часть в список left,
/// а другую в список right.
let rec splitList list left right =
        match list with
        | [] -> (left, right)
        | a::b::tail -> splitList tail (a::left) (b::right)
        | [a] -> (a::left, right)

/// Объединяет 2 отсортированных списка в один отсортированный.
let rec merge list1 list2 accumList =
    match (list1, list2) with
    | (h1::tail1, h2::tail2) ->
        if h1 <= h2 then
            merge tail1 list2 (h1::accumList)
        else
            merge list1 tail2 (h2::accumList)
    | (h::tail,[]) | ([], h::tail) -> merge tail [] (h::accumList)
    | ([], []) -> List.rev accumList

/// Функция сортирует заданный список слиянием.
let rec mergeSort list =
    match list with
    | [] -> []
    | [_] -> list
    | _ -> 
        match splitList list [] [] with
        | (left, right) -> 
            merge (mergeSort left) (mergeSort right) []
