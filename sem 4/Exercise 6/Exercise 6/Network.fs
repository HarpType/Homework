namespace NetworkInteraction

module Network =

    open Computer

    /// Класс моделирует работу сети.
    type Network(computers : array<Computer>, adjacencyMatrix : array<array<int>>) =
        let mutable computers = computers

        let mutable computersCount = computers.Length

        /// Проверяет матрицу смежности на корректность
        let correctAdjacencyMatrix (adjacencyMatrix : array<array<int>>) =
            if adjacencyMatrix.Length <> computersCount then
                failwith "Adjacency matrix exception."
            else
                for i in 0 .. adjacencyMatrix.Length do
                    if adjacencyMatrix.[i].Length <> computersCount then
                        failwith "Adjacency matrix exception."

                adjacencyMatrix

        let adjacencyMatrix = correctAdjacencyMatrix adjacencyMatrix

        /// private-метод. Создаёт список всех заражённых компьютеров.
        let createInjectedList = 
            let rec accumCreateInjectedList (injectedList : List<int>) index =
                if index >= computers.Length then
                    injectedList
                else 
                    if computers.[index].IsInfected then
                        accumCreateInjectedList (index :: injectedList) (index + 1)
                    else
                        accumCreateInjectedList injectedList (index + 1)

            accumCreateInjectedList [] 0

        /// private-метод. Моделирует процесс заражения для компьютеров, связанных с главным
        /// по матрице смежности.
        let calculateInfection infectedComputerIndex = 
            for i in 0 .. adjacencyMatrix.[infectedComputerIndex].Length do
                if (adjacencyMatrix.[infectedComputerIndex].[i] = 1) 
                        && (not computers.[i].IsInfected) then
                    let rnd = System.Random()
                    let randomNumber = rnd.Next(0, 99)
                    if computers.[i].OperatingSystem.InfectionChance > randomNumber then
                        computers.[i].IsInfected <- true

        member this.Computers
            with get() = computers

        /// Производит шаг моделирования. 
        member this.Step ()=
            let injectedList = createInjectedList

            List.iter (fun (index) -> calculateInfection index) injectedList

