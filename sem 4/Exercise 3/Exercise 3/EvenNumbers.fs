module CountingEvenNumbers =

    /// Функция подсчитывает количество чётных чисел в списке. 
    /// В основе функции - Seq.map.
    let countingWithMap list =
        list |> Seq.map(fun x -> ((x % 2) + 1) % 2) |> 
            Seq.fold(fun acc elem -> acc + elem) 0

    /// Подсчитывает количество чётных чисел.
    /// В основе - Seq.filter.
    let countingWithFilter list =
        list |> Seq.filter(fun x -> x % 2 = 0) |> Seq.length

    /// Подсчитывает количество чётных чисел.
    /// В основе - Seq.fold.
    let countingWithFold list =
        list |> Seq.fold(fun acc elem -> acc + ((elem % 2) + 1) % 2) 0
