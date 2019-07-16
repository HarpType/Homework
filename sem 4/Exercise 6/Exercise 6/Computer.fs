namespace NetworkInteraction

module Computer =
    
    open OperatingSystem

    /// Класс, представляющий компьютер.
    type Computer(operatingSystem : OperatingSystem) = 
        member val OperatingSystem = operatingSystem with get,set
        member val IsInfected = false with get,set