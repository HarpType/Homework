namespace Tests

module BinTreeMapTest =
    open NUnit.Framework
    open FsUnit
    open BinTree

    let testBinTree1 = Node(5, Node(4, EmptyNode, EmptyNode), Node(6, EmptyNode, EmptyNode))
    let resultBinTree1 = Node(25, Node(16, EmptyNode, EmptyNode), Node(36, EmptyNode, EmptyNode))

    let testBinTree2 = Node(4, Node(5, 
                                    Node(6, EmptyNode, EmptyNode), 
                                    Node(8, EmptyNode, EmptyNode)), 
                               Node(3, 
                                    Node(9, EmptyNode, EmptyNode),
                                    EmptyNode))
    let resultBinTree2 = Node(0, Node(1, 
                                        Node(0, EmptyNode, EmptyNode),
                                        Node(0, EmptyNode, EmptyNode)),
                                    Node(1,
                                        Node(1, EmptyNode, EmptyNode),
                                        EmptyNode))

    [<Test>]
    let ``Mapping x^2 with EmptyTree equals EmptyTree`` ()=
        BinTree.map (EmptyNode) (fun(x) -> x * x) |> should equal (EmptyNode:BinTree<int32>)

    [<Test>]
    let ``Mapping x^2 with binTree`` ()=
        BinTree.map testBinTree1 (fun x -> x * x) |> should equal resultBinTree1

    [<Test>]
    let ``Mapping mod with binTree`` ()=
        BinTree.map testBinTree2 (fun x -> x % 2) |> should equal resultBinTree2