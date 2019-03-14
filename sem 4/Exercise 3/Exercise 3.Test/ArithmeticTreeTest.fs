namespace Tests

module ArithmeticTreeTest =
    open NUnit.Framework
    open FsUnit
    open BinTree

    let testArithmeticTree1 = Node("+", EmptyNode, Node("3", EmptyNode, EmptyNode))
    let testArithmeticTree2 = Node("+", 
                                        Node("-", 
                                            Node("8", EmptyNode, EmptyNode),
                                            Node("3", EmptyNode, EmptyNode)),
                                        Node("*", 
                                            Node("/",
                                                Node("16", EmptyNode, EmptyNode),
                                                Node("4", EmptyNode, EmptyNode)),
                                            Node("2", EmptyNode, EmptyNode)))
    let testArithmeticTree3 = Node("/", Node("3", EmptyNode, EmptyNode), Node("0", EmptyNode, EmptyNode))
    
    [<Test>]
    let ``Wrong arithmetic tree should return None`` ()=
        testArithmeticTree1 |> calculateArithmeticTree |> should equal None

    [<Test>]
    let ``Calculation testArithemticTree2 equals 13`` ()=
        testArithmeticTree2 |> calculateArithmeticTree |> Option.get |> should equal 13.0

    [<Test>]
    let ``Dividing float by zero should return infinity`` ()=
        testArithmeticTree3 |> calculateArithmeticTree |> Option.get |> should equal infinity