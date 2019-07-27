namespace Lazy

    open System.Threading

    // Описывает работу многопоточных ленивых вычислений.
    // 'a -- тип, возвращаемый ленивым вычислением.
    // supplier -- лямбда-функция, лежащая в основе ленивого вычисления.
    type SafeMultipleThreadLazy<'a> (supplier: unit -> 'a) =

        let monitor = new obj()

        // Если значение ленивого вычисления уже найдено, то равен true,
        // false в противном случае.
        let mutable hasValue = false

        // Хранит информацию о вычислении лямбда-функции.
        let mutable optionValue: option<'a> = None

        interface ILazy<'a> with

            // Реализация метода ленивого вычисления.
            // Гарантирует корректную работу в многопоточном режиме.
            member this.Get () =
                if Volatile.Read(ref hasValue) |> not then
                     lock monitor (fun () -> 
                        if Volatile.Read(ref hasValue) |> not then
                            do optionValue <- Some(supplier())
                            do hasValue <- true
                    )

                optionValue.Value
