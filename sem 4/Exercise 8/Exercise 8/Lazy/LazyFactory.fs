namespace Lazy

module LazyFactory =

    open Lazy
    
    open SingleThreadLazy
    open SafeMultipleThreadLazy
    open LockFreeMultipleThreadLazy
    open ILazy

    // Класс создаёт объекты ленивых вычислений.
    type LazyFactory =

        // Метод возвращает объект ленивого вычисления, предоставляющий корректную
        // работу в однопоточном режиме.
        // supplier -- лямбда-функция, лежащая в основе ленивого вычисления. 
        static member CreateSingleThreadedLazy (supplier: unit -> 'a) =
            SingleThreadLazy(supplier)

        // Метод возвращает объект ленивого вычисления, предоставляющий корректную 
        // работу в многопоточном режиме.
        // supplier -- лежит в основе ленивого вычисления.
        static member CreateSafeMultipleThreadLazy (supplier: unit -> 'a) =
            SafeMultipleThreadLazy(supplier)

        // Метод возвращает объект lock-free ленивого вычисления, предоставляющий
        // корректную работу в многопоточном режиме.
        // supplier -- лежит в основе ленивого вычисления.
        static member CreateLockFreeMultipleThreadLazy(supplier: unit -> 'a) =
            LockFreeMultipleThreadLazy(supplier)
