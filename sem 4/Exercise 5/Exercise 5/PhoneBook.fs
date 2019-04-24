module PhoneBook

open System

    type PhoneRecord = {Name:string; PhoneNumber:string}

    type PhoneRecordTree = 
        | Node of PhoneRecord * PhoneRecordTree * PhoneRecordTree
        | EmptyNode

    /// Проверяет корректность введённого телефонного номера.
    let isPhoneNumber (phoneNumber:string) =
        let rec isPhoneNumberAccum (phoneList:list<char>) i =
            if i = 11 then
                phoneList.IsEmpty
            else
                match phoneList with
                | h :: t -> 
                    if (h >= '0') && (h <= '9') then
                        isPhoneNumberAccum t (i+1)
                    else 
                        false
                | [] -> false

        isPhoneNumberAccum (Seq.toList phoneNumber) 0


    /// Вставляет запись (имя и телефон) в двоичное дерево. 
    /// Ключ этого дерева -- телефонный номер. Предполагается, что телефонный номер -- 
    /// уникальное значение, имя -- не уникальное.
    /// В случае вставки записи с уже существующим номером, происходит его обновление.
    let rec addPhoneRecord phoneRecordTree newPhoneRecord =
        match phoneRecordTree with
        | EmptyNode -> Node(newPhoneRecord, EmptyNode, EmptyNode)
        | Node(phoneRecord, leftNode, rightNode) ->
            let comparisonResult = String.Compare (newPhoneRecord.PhoneNumber, phoneRecord.PhoneNumber)
            if comparisonResult = 0 then
                Node(newPhoneRecord, leftNode, rightNode)
            elif comparisonResult < 0 then
                Node(phoneRecord, leftNode, addPhoneRecord rightNode newPhoneRecord)
            else 
                Node(phoneRecord, addPhoneRecord leftNode newPhoneRecord, rightNode)


    /// Находит имя по номеру телефона.
    let rec findByPhoneNumber phoneRecordTree phoneNumber =
        match phoneRecordTree with
        | EmptyNode -> None
        | Node(phoneRecord, leftNode, rightNode) ->
            let comparisonResult = String.Compare (phoneNumber, phoneRecord.PhoneNumber)
            if comparisonResult = 0 then
                Some(phoneRecord.Name)
            elif comparisonResult < 0 then
                findByPhoneNumber rightNode phoneNumber
            else
                findByPhoneNumber leftNode phoneNumber


    /// Тип, содержащий информацию об обходе дерева в глубину.
    type ContinuationStep =
        | Finished
        | Step of  PhoneRecord * (unit -> ContinuationStep)


    /// Формирует шаги обхода дерева в глубину.
    let rec linearizePhoneTree phoneRecordTree cont = 
        match phoneRecordTree with
        | EmptyNode -> cont()
        | Node(record, leftNode, rightNode) ->
            Step(record, (fun () -> linearizePhoneTree leftNode (fun () ->
                            linearizePhoneTree rightNode cont)))

    
    /// Запускает обход дерева в глубину, применяет функцию func 
    /// к каждой записи в узле. Возвращает последовательность из значений, которая удовлетворяет
    /// ограничениям, предоставленные пользователем.
    let iterPhoneTree phoneRecordTree checkFunc =
        let steps = linearizePhoneTree phoneRecordTree (fun () -> Finished)

        let rec processSteps step = 
            seq {
            match step with
            | Finished -> ()
            | Step(record, getNext) ->
                if checkFunc record then
                    yield record
                yield! processSteps(getNext()) }

        processSteps steps

    let findByName phoneRecordTree name = iterPhoneTree phoneRecordTree (fun record -> if record.Name = name then true else false)


    /// Функция осуществляет общение с пользователем через консоль.
    let rec StepPhoneBook phoneRecordTree = 
        printfn "Enter the command (exit, add, find-by-phone, find-by-name, write-to-console, save-to-file, load-from-file):"
        let command = Console.ReadLine()

        match command with 
        | "exit" -> 0
        | "add" -> 
            printfn "Enter a name:"
            let name = Console.ReadLine()
            printfn "Enter a phone number:"
            let newPhoneNumber = Console.ReadLine()

            if isPhoneNumber newPhoneNumber then
                addPhoneRecord phoneRecordTree {Name=name; PhoneNumber=newPhoneNumber} |> StepPhoneBook
            else 
                printfn "Wrong phone number!"
                StepPhoneBook phoneRecordTree
        | "find-by-phone" ->
            printfn "Enter a phone number:"
            let phoneNumber = Console.ReadLine()

            if isPhoneNumber phoneNumber then
                match findByPhoneNumber phoneRecordTree phoneNumber with 
                | Some(name) -> printfn "%s" name
                | None -> printfn "No such phone number"

                StepPhoneBook phoneRecordTree
            else
                printfn "Wrong phone number!"
                StepPhoneBook phoneRecordTree
        | _ -> 0
    