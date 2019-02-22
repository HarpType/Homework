// Функция обращает заданный список list за линейное время
let reverse (list:list<_>) = 
    let rec accumReverse (list1:list<_>) (list2:list<_>) =
        if list1.Length = 0 then
            list2
        else
            accumReverse list1.Tail (list1.Head :: list2)
    accumReverse list []


(* Функция принимает на вход целые числа n и m и возвращает список вида 
[2^n; 2^(n+1); ...; 2^(n+m)] или пустой список, если хотя бы один 
из аргументов отрицателен *)

let listPower n m = 
    if n < 0 || m < 0 then
        []
    else
        let rec twoToPowerOfn n i acc = 
            if n = i then
                acc
            else
                twoToPowerOfn n (i + 1) (acc * 2)
    
        let rec listPowerOfm m i list = 
            if i = m then
                reverse list
            else listPowerOfm m (i + 1) ((list.Head * 2) :: list)
    
        listPowerOfm m 0 [twoToPowerOfn n 0 1] 

