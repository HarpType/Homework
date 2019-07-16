namespace Lazy

module LazyFactory =

    // Класс создаёт объекты ленивых вычислений.
    type LazyFactory =

        // Метод возвращает объект ленивого вычисления, предоставляющий корректную
        // работу в однопоточном режиме.
        // supplier -- лямбда-функция, лежащая в основе ленивого вычисления. 
        static member CreateSingleThreadedLazy (supplier: unit -> 'a) =
