namespace NetworkInteraction

module Network =

    open Computer

    /// Класс моделирует работу сети.
    type Network(computers : array<Computer>, adjacencyMatrix : array<array<int>>) =

        /// Проверяет матрицу смежности на корректность
        let checkAdjacencyMatrix (adjacencyMatrix : array<array<int>>) =
            if adjacencyMatrix.Length <> computers.Length then
                failwith "Adjacency matrix exception."
            else
                for i in 0 .. (adjacencyMatrix.Length - 1) do
                    if adjacencyMatrix.[i].Length <> computers.Length then
                        failwith "Adjacency matrix exception."

                adjacencyMatrix

        let adjacencyMatrix = 
            do (checkAdjacencyMatrix adjacencyMatrix |> ignore)
            adjacencyMatrix

        /// private-метод. Создаёт список всех заражённых компьютеров.
        let createInfectedList () = 
            let rec accumCreateInfectedList (infectedList : List<int>) index =
                if index >= computers.Length then
                    infectedList
                else 
                    if computers.[index].IsInfected then
                        accumCreateInfectedList (index :: infectedList) (index + 1)
                    else
                        accumCreateInfectedList infectedList (index + 1)

            accumCreateInfectedList [] 0

        let rnd = System.Random()
    
        /// private-метод. Моделирует процесс заражения для компьютеров, связанных с главным
        /// по матрице смежности.
        let calculateInfection infectedComputerIndex = 
            for i in 0 .. (adjacencyMatrix.[infectedComputerIndex].Length - 1) do
                if (adjacencyMatrix.[infectedComputerIndex].[i] = 1) 
                        && (not computers.[i].IsInfected) then
                    let randomNumber = rnd.Next(0, 99)
                    if computers.[i].OperatingSystem.InfectionChance > randomNumber then
                        computers.[i].IsInfected <- true

        member this.Computers
            with get() = computers

        /// Производит шаг моделирования. 
        member this.Step =
            let infectedList = createInfectedList()

            List.iter (fun (index) -> calculateInfection index) infectedList

        /// Возвращает список инфицированных компьютеров.
        member this.InfectedComputers =
            createInfectedList()

