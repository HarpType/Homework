module BinTree

    /// Тип описывает структуру двоичного дерева.
    type BinTree<'a> =
        | Node of 'a * BinTree<'a> * BinTree<'a>
        | EmptyNode

    (* Блок задачи 2: map для бинарных деревьев. *)

    /// Позволяет применить заданную функцию func ко всем элементам бинарного дерева.
    /// Возвращает новое дерево, каждый элемент которого - результат 
    /// применения заданной функции.
    let rec map binTree func =
        match binTree with
        | EmptyNode -> EmptyNode
        | Node(x, l, r) -> Node(func x, map l func, map r func)

    (* Блок задачи 3: вычисление значения по дереву выражения. *)

    /// Функция вычисляет значение по заданному дереву арифметического выражения.
    let calculateArithmeticTree arithmeticTree =
        let rec linearizeArithmeticTree arithmeticTree cont = 
            match arithmeticTree with
            | Node(x, l, r) ->
                match x with
                | "+" -> linearizeArithmeticTree l (fun(a) -> linearizeArithmeticTree r (fun(b) -> cont(a + b)))
                | "-" -> linearizeArithmeticTree l (fun(a) -> linearizeArithmeticTree r (fun(b) -> cont(a - b)))
                | "*" -> linearizeArithmeticTree l (fun(a) -> linearizeArithmeticTree r (fun(b) -> cont(a * b)))
                | "/" -> linearizeArithmeticTree l (fun(a) -> linearizeArithmeticTree r (fun(b) -> cont(a / b)))
                | _ -> 
                    try 
                        cont(x |> float)
                    with
                    | :? System.FormatException -> None
            | EmptyNode -> None
        linearizeArithmeticTree arithmeticTree (fun(a) -> Some(a))
