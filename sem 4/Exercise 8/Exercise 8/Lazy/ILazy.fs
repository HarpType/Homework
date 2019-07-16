namespace Lazy

module ILazy =

    // Интерфейс леинвых вычислений. 
    // 'a -- тип вычисления, возвращаемого ленивой функцией.
    type ILazy<'a> =

        // Представляет результат ленивого вычисления.
        abstract member Get: unit -> 'a


