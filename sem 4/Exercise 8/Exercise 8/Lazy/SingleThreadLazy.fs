namespace Lazy

module SingleThreadLazy =

    open ILazy

    // Описывает работу однопоточных ленивых вычислений.
    // 'a -- тип, возвращаемый ленивым вычислением.
    // supplier -- лямбда-функция, лежащая в основе ленивого вычисления.
    type SingleThreadLazy<'a> (supplier: unit -> 'a) =
        
        // Хранит информацию о вычислении лямбда-функции supplier.
        // Имеет значение None до вычисления лямбда-функции и вычисленное значение,
        // обёрнутое в Some после.
        let mutable (optionValue: option<'a>) = None

        interface ILazy<'a> with

            // Реализация абстрактного метода для однопоточного ленивого вычисления.
            member this.Get () =
                match optionValue with
                | Some(value) -> value
                | None ->
                    do optionValue <- Some(supplier())
                    optionValue.Value
