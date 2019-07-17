namespace Lazy

module LockFreeMultipleThreadLazy =

    open ILazy
    open System.Threading

    // Описывает работу lock-free многопоточных ленивых вычислений.
    // 'a -- тип, возвращаемый ленивым вычислением.
    // supplier -- лямбда-функция, лежащая в основе ленивого вычисления.
    type SafeMultipleThreadLazy<'a> (supplier: unit -> 'a) =

        let mutable isHasValue = 0

        // Хранит информацию о вычислении лямбда-функции.
        let mutable (optionValue: option<'a>) = None

        interface ILazy<'a> with

            // Реализация lock-free метода ленивого вычисления.
            // Гарантирует корректную работу в многопоточном режиме.
            member this.Get () =
                if isHasValue = 1 then
                    optionValue.Value
                else
                    let startIsHasValue = 0
                    let intermediateOptionValue = Some(supplier())

                    if 0 = Interlocked.CompareExchange(ref isHasValue, 1, startIsHasValue) then
                        do optionValue <- intermediateOptionValue
                        optionValue.Value
                    else 
                        optionValue.Value
