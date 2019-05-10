module HashTable

    let MAX_CELL_VALUE = 255

    /// Тип элементов, которые хранятся в hash-таблице.
    type HashTableItem = {Key: int; Value: string}

    type HashTable(hashFunc : (int -> byte)) = 
        let mutable table = [| for _ in 0 .. MAX_CELL_VALUE -> List.empty<HashTableItem> |]

        /// private-метод, реализующий удаление итема из заданной ячейки. 
        /// Возвращает новый список, без заданного элемента.
        let rec Remove list item =
            match list with
            | h :: t ->
                if h = item then
                    t
                else h :: Remove t item
            | [] -> []

        /// Добавляет элемент в хеш-таблицу. Может хранить дубликаты.
        member this.Insert (item:HashTableItem) =
            let cellNumber = item.Key |> hashFunc
            table.[cellNumber |> int] <- item :: table.[cellNumber |> int]

        /// Проверяет, находится ли данный элемент в хеш-таблице.
        member this.Contains (item:HashTableItem) =
            let cellNumber = item.Key |> hashFunc
            if List.contains item table.[cellNumber |> int] then
                true
            else 
                false

        /// Удаляет заданный элемент из таблицы (если он находится в нём).
        member this.Delete (item:HashTableItem) = 
            let cellNumber = item.Key |> hashFunc
            table.[cellNumber |> int] <- Remove  table.[cellNumber |> int] item
            



            
