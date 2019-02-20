// Функция обращает заданный список list
let reverse (list:list<_>) = 
    let rec acc_reverse (list1:list<_>) (list2:list<_>) length i =
        if length = i then
            list2
        else
            acc_reverse list1 (list2 @ [list1.Item(length-i-1)]) length (i+1)
    acc_reverse list [] list.Length 0
