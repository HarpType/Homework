namespace Lazy

module SingleThreadLazy =

    open ILazy

    // Описывает работу однопоточных ленивых вычислений.
    // 'a -- тип, возвращаемый ленивым вычислением.
    // supplier -- лямбда-функция, лежащая в основе ленивого вычисления.
    type SingleThreadLazy<'a> (supplier: unit -> 'a) =

        let mutable isHasValue = false

        interface ILazy<'a> with
            member Get () =
                if isHasValue then
