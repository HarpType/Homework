(* Функция выдаёт элемент последовательности чисел Фибоначчи
 по заданной позиции n *)
let fibonacci n =
    let rec accumFibonacci n i acc1 acc2 =
            if n = i then
                acc1
            elif n > 0 then
                accumFibonacci n (i + 1) acc2 (acc1 + acc2)
            else
                accumFibonacci n (i - 1) (acc2 - acc1) acc1
    accumFibonacci n 0 0 1           
