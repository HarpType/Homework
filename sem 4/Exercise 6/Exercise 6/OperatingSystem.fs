namespace NetworkInteraction

module OperatingSystem =

    /// Класс операционной системы. В качестве аргумента конструктора -- вероятность заражения (от 0 до 100)
    type OperatingSystem(infectionChance) =
        /// private-метод. Возвращает 100, если шанс больше 100 и 0, если шанс меньше 0
        let getCorrectChance chance = 
            if chance > 100 then
                100
            elif chance < 0 then
                0
            else
                chance

        member val InfectionChance = getCorrectChance infectionChance with get
