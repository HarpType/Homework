// Функция считает факториал от числа n
let fact n = 
    if n < 0 then
        0
    else
        let rec accFact n acc =
            match n with 
            | 0 -> 1
            | 1 -> acc
            | _ -> accFact (n-1) acc*(n-1)
        accFact n n
