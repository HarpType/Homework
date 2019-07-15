namespace Tests

module PrimeNumbersTest =
    open NUnit.Framework
    open FsUnit
    open SequenceOfPrimeNumbers

    let takenPrimeNumbers1 = Seq.take 5 primeNumbers
    let takenPrimeNumbers2 = Seq.take 45 primeNumbers

    let testList2 = [2; 3; 5; 7; 11; 13; 17; 19; 23; 29; 31; 37; 41; 43; 47; 
                    53; 59; 61; 67; 71; 73; 79; 83; 89; 97; 101; 103; 107; 
                    109; 113; 127; 131; 137; 139; 149; 151; 157; 163; 167; 
                    173; 179; 181; 191; 193; 197]

    [<Test>]
    let ``First 5 prime numbers`` ()=
        takenPrimeNumbers1 |> Seq.toList |> should equal [2; 3; 5; 7; 11]

    [<Test>]
    let ``First 45 prime numbers`` ()=
        takenPrimeNumbers2 |> Seq.toList |> should equal testList2