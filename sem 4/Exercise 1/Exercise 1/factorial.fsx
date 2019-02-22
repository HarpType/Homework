// Функция считает факториал от числа n
let factorial n = 
    if n < 0 then
        0
    else
        let rec accFactorial n acc =
            match n with 
            | 0 -> 1
            | 1 -> acc
            | _ -> accFactorial (n - 1) acc * (n - 1)
        accFactorial n n

