(* Функция выдаёт элемент последовательности чисел Фибоначчи
 по заданной позиции n *)
let fibonacci n =
    let rec accFibonacci n i acc1 acc2 =
            if n = i then
                acc1
            elif n > 0 then
                accFibonacci n (i+1) acc2 (acc1+acc2)
            else
                accFibonacci n (i-1) (acc2-acc1) acc1
    accFibonacci n 0 0 1           
