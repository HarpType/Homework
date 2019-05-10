module HashTable

    let MAX_CELL_VALUE = 255

    type HashTableItem = {Key: int; Value: string}

    type HashTable(hashFunc : (int -> byte)) = 
        let mutable table = [| for _ in 1.. MAX_CELL_VALUE -> List.empty<HashTableItem> |]

        /// private-метод, реализующий удаление итема из заданной ячейки. 
        /// Возвращает новый список, без заданного элемента.
        let rec remove list item =
            match list with
            | h :: t ->
                if h = item then
                    t
                else h :: remove t item
            | [] -> []

        /// Добавляет элемент в хеш-таблицу. Может хранить дубликаты.
        member this.insert (item:HashTableItem) =
            let cellNumber = item.Key |> hashFunc
            table.[cellNumber |> int] <- item :: table.[cellNumber |> int]

        /// Проверяет, находится ли данный элемент в хеш-таблице.
        member this.contains (item:HashTableItem) =
            let cellNumber = item.Key |> hashFunc
            if List.contains item table.[cellNumber |> int] then
                true
            else 
                false

        /// Удаляет заданный элемент из таблицы (если он находится в нём).
        member this.delete (item:HashTableItem) = 
            let cellNumber = item.Key |> hashFunc
            table.[cellNumber |> int] <- remove  table.[cellNumber |> int] item
            



            
