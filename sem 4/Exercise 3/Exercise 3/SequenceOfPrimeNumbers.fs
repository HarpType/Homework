module SequenceOfPrimeNumbers
    
    /// Функция, предоставляющая округлённый корень числа x.
    let intSqrt x = 
        System.Math.Round(x |> float |> sqrt) |> int

    /// Функция проверяет число на простоту.
    let isPrime number = 
        let rec iterIsPrime number sqrt i = 
            if i > sqrt then
                true
            elif number % i = 0 then
                false
            else    
                iterIsPrime number sqrt (i + 2)
        
        if number = 2 then
            true
        elif number % 2 = 0 then
            false
        else 
            iterIsPrime number (intSqrt number) 3

    /// бесконечная последовательность простых чисел.
    let primeNumbers = 
        let rec createSeqPrime n =
            seq {if isPrime n then yield n; yield! createSeqPrime (n + 1) else yield! createSeqPrime (n + 1)}

        createSeqPrime 2
