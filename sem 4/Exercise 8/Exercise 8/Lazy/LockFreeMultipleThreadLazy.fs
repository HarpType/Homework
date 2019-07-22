namespace Lazy

    open System.Threading

    // Описывает работу lock-free многопоточных ленивых вычислений.
    // 'a -- тип, возвращаемый ленивым вычислением.
    // supplier -- лямбда-функция, лежащая в основе ленивого вычисления.
    type LockFreeMultipleThreadLazy<'a> (supplier: unit -> 'a) =

        // Указывает, пытается ли изменить какой-нибудь из потоков значение optionValue
        let usingResource = ref 0

        // Хранит информацию о вычислении лямбда-функции.
        let mutable (optionValue: option<'a>) = None
        
        interface ILazy<'a> with

            // Реализация lock-free метода ленивого вычисления.
            // Гарантирует корректную работу в многопоточном режиме.
            member this.Get () =
                if optionValue.IsNone then
                    let intermediateOptionValue = Some(supplier())

                    let previousUsingResource = Interlocked.Exchange(&usingResource.contents, 1)
                    if previousUsingResource = 0 then
                        do optionValue <- intermediateOptionValue
                    else
                        while optionValue.IsNone do
                            Volatile.Read<option<'a>>(ref optionValue) |> ignore

                optionValue.Value
