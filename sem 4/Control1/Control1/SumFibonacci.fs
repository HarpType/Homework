module SumFibonacci


// Функция возвращает сумму чётных чисел фибоначчи, не превосходящих потолка sill.
let sumFibonacci sill =
    // Возвращает элемент последовательности чисел фибоначчи по заданной позиции n
    let fibonacci n =
        let rec accumFibonacci n i acc1 acc2 =
                if n = i then
                    acc1
                else
                    accumFibonacci n (i + 1) acc2 (acc1 + acc2)
        accumFibonacci n 0 0 1           

    // Рекурсивно считает сумму всех чётных чисел последовательности фибоначчи,
    // не превосходящих sill.
    let rec accumSumFibonacci i accumSum =
        let iFibonacci = fibonacci(i)
        if iFibonacci > sill then
            accumSum
        elif iFibonacci % 2 = 0 then
            accumSumFibonacci (i+1) (accumSum + iFibonacci)
        else 
            accumSumFibonacci (i+1) accumSum

    accumSumFibonacci 1 0
