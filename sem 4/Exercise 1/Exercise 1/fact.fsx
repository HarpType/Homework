// Функция считает факториал от числа x
let fact x = 
    if x < 0 then
        0
    else
        let rec accFact x acc =
            if x <= 1 then
                acc
            else
                accFact (x-1) acc*(x-1)
        accFact x x
