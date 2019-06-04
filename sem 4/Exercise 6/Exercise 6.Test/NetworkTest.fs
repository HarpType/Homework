namespace Tests

module NetworkTest =
    open NUnit.Framework
    open FsUnit
    open NetworkInteraction.OperatingSystem
    open NetworkInteraction.Computer
    open NetworkInteraction.Network

    [<Test>]
    let ``100 percent infection test`` ()=
        let badOC = OperatingSystem(100)

        let comp1 = Computer(badOC)
        let comp2 = Computer(badOC)
        let comp3 = Computer(badOC)
        let comp4 = Computer(badOC)
        comp1.IsInfected <- true

        /// Компьютеры объединены в цепочку от первого заражённого.
        let network = Network([|comp1; comp2; comp3; comp4|], 
                        [|[|0; 1; 0; 0|]; [|1; 0; 1; 0|]; [|0; 1; 0; 1|]; [|0; 0; 1; 0|]|])

        for i in 2 .. 4 do
            network.Step
            network.InfectedComputers.Length |> should equal i

    [<Test>]
    let ``0 percent infection test`` ()=
        let proOC = OperatingSystem(0);

        let comp1 = Computer(proOC)
        let comp2 = Computer(proOC)
        let comp3 = Computer(proOC)
        let comp4 = Computer(proOC)
        comp1.IsInfected <- true

        /// Компьютеры объединены каждый с каждым, один компьютер заражён.
        let network = Network([|comp1; comp2; comp3; comp4|], 
                        [|[|0; 1; 1; 1|]; [|1; 0; 1; 1|]; [|1; 1; 0; 1|]; [|1; 1; 1; 0|]|])

        for _ in 1 .. 100 do
            network.Step

        network.InfectedComputers.Length |> should equal 1

    [<Test>]
    let exceptionTest ()=
        let proOC = OperatingSystem(0);

        let comp1 = Computer(proOC)

        (fun () -> Network([|comp1|], [|[|0; 1; 1; 1|]; [|1; 0; 1; 1|]; [|1; 1; 0; 1|]; [|1; 1; 1; 0|]|]) 
                |> ignore) |> should throw typeof<System.Exception>