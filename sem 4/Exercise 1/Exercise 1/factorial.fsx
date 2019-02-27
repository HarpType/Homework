// Функция считает факториал от числа n
let factorial n = 
    if n < 0 then
        0
    else
        let rec accumFactorial n acc =
            match n with 
            | 0 -> 1
            | 1 -> acc
            | _ -> accumFactorial (n - 1) acc * (n - 1)
        accumFactorial n n

