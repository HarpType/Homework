(* Функция принимает на вход целые числа n и m и возвращает список вида 
[2^n; 2^(n+1); ...; 2^(n+m)] или пустой список, если хотя бы один 
из аргументов отрицателен *)
let arrayPower n m = 
    if n < 0 || m < 0 then
        []
    else
        let rec accnPower n i acc = 
            if n = i then
                acc
            else
                accnPower n (i+1) (acc*2)
    
        let rec accmListPower m i list = 
            if i = m then
                list
            else accmListPower m (i+1) (list @ [list.Item(list.Length-1) * 2])
    
        accmListPower m 0 [accnPower n 0 1]

