module BinTree

    /// Тип описывает структуру двоичного дерева.
    type BinTree<'a> =
        | Node of 'a * BinTree<'a> * BinTree<'a>
        | EmptyNode

    /// Позволяет применить заданную функцию func ко всем элементам бинарного дерева.
    /// Возвращает новое дерево, каждый элемент которого - результат 
    /// применения заданной функции.
    let rec map binTree func =
        match binTree with
        | EmptyNode -> EmptyNode
        | Node(x, l, r) -> Node(func x, map l func, map r func)
