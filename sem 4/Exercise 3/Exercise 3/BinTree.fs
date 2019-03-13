module BinTree

    /// Тип описывает структуру двоичного дерева.
    type BinTree<'a> =
        | Node of 'a * BinTree<'a> * BinTree<'a>
        | EmptyNode

    /// Тип используется при обходе дерева.
    type ContinuationStep<'a> =
        | Finished
        | Step of 'a * (unit -> ContinuationStep<'a>)
    
    /// Переводит объект типа BinTree в объект типа ContinuationStep.
    let calculateSteps binTree =
        let rec linearize binTree cont =
            match binTree with
            | EmptyNode -> cont()
            | Node(x, l, r) -> 
                Step(x, (fun() -> linearize l (fun() -> linearize r cont)))
        linearize binTree (fun() -> Finished)

    /// Вставляет элемент item в бинарное дерево binTree.
    let rec insertItem binTree item =
        match binTree with
        | EmptyNode -> Node (item, EmptyNode, EmptyNode)
        | Node(x, l, r) as node ->
            if item = x then node
            else if item < x then Node(x, insertItem l item, r)
            else Node (x, l, insertItem r item)

    /// Позволяет применить заданную функцию func ко всем элементам бинарного дерева.
    /// Возвращает новое дерево, каждый элемент которого - результат 
    /// применения заданной функции.
    let map binTree func = 
        let rec accumMap step accumTree func = 
            match step with
            | Finished -> accumTree
            | Step(x, getNext) ->
                accumMap (getNext()) (insertItem accumTree (func(x))) func
        accumMap (calculateSteps binTree) EmptyNode func
