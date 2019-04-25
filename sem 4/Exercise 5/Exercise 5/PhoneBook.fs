module PhoneBook

open System
open System.IO

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
        if isPhoneNumber newPhoneRecord.PhoneNumber |> not then
            failwith "The wrong phone number."

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
    /// ограничениям, предоставленные функцией func.
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


    /// Поиск телефона по имени. Возвращает список всех телефонов, имя записей которых совпадает с указанным.
    let findByName phoneRecordTree name = 
            let seqRecords = iterPhoneTree phoneRecordTree (fun record -> if record.Name = name then true else false)
            let listPhones = Seq.map (fun phoneRecord -> phoneRecord.PhoneNumber) seqRecords

            Seq.toList listPhones

    /// Сохраняет содержимое дерева с телефонными записями в файл с именем fileName.
    let savePhoneBookToFile phoneRecordTree fileName = 
        let listPhoneRecord = iterPhoneTree phoneRecordTree (fun phoneRecord -> true)
                                |> Seq.map (fun phoneRecord -> phoneRecord.Name + "|" + phoneRecord.PhoneNumber) |> Seq.toList

        File.WriteAllLines(fileName, listPhoneRecord)

    /// Выгружает данные из файла.
    let loadPhoneBookFromFile fileName =
        let phoneRecords = File.ReadLines fileName |> Seq.toList

        if phoneRecords.IsEmpty then
            EmptyNode
        else 
            let phoneRecordTree = phoneRecords |> List.map (fun record -> {Name=record.Split('|').[0]; PhoneNumber=record.Split('|').[1]})
        
            phoneRecordTree |> Seq.fold (fun accum phoneRecord -> addPhoneRecord accum phoneRecord) EmptyNode



    /// Функция осуществляет общение с пользователем через консоль.
    let rec stepPhoneBook phoneRecordTree = 
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
                addPhoneRecord phoneRecordTree {Name=name; PhoneNumber=newPhoneNumber} |> stepPhoneBook
            else 
                printfn "Wrong phone number!"
                stepPhoneBook phoneRecordTree
        | "find-by-phone" ->
            printfn "Enter a phone number:"
            let phoneNumber = Console.ReadLine()

            if isPhoneNumber phoneNumber then
                match findByPhoneNumber phoneRecordTree phoneNumber with 
                | Some(name) -> printfn "%s" name
                | None -> printfn "No such phone number"

                stepPhoneBook phoneRecordTree
            else
                printfn "Wrong phone number!"
                stepPhoneBook phoneRecordTree
        | "find-by-name" ->
            printfn "Enter a name:"
            let name = Console.ReadLine()

            let phones = findByName phoneRecordTree name
            if phones.IsEmpty |> not then
                Seq.iter (fun phone -> printf "%s " phone) phones
                printfn ""
                
                stepPhoneBook phoneRecordTree
            else
                printfn "There is no this name in the phone book!"

                stepPhoneBook phoneRecordTree
        | "save-to-file" ->
            printfn "Enter a file name:"
            let fileName = Console.ReadLine()

            savePhoneBookToFile phoneRecordTree fileName

            stepPhoneBook phoneRecordTree
        | "load-from-file" ->
            printfn "Enter a file name:"
            let fileName = Console.ReadLine()

            try
                loadPhoneBookFromFile fileName |> stepPhoneBook
            with
                | Failure(msg) -> printf "%s" msg; stepPhoneBook phoneRecordTree

        | _ -> 
            printfn "Wrong command."

            stepPhoneBook phoneRecordTree
    