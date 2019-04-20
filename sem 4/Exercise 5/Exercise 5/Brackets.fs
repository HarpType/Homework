module Brackets
    
    /// Возвращает true, если открывающий и закрывающий скобки
    /// одного типа. false в противном случае.
    let isPairBrackets openedBracket closedBracket =
        if (openedBracket = '(') && (closedBracket = ')') then
            true
        elif (openedBracket = '[') && (closedBracket = ']') then
            true
        elif (openedBracket = '{') && (closedBracket = '}') then
            true
        else false

    /// Возвращает true, если строка скобок корректна и false в противном случае.
    /// Вызывает исключение, если в строке присутствуют посторонние символы.
    let isCorrectBrackets bracketsString =
        let rec checkBracketsWithStack (bracketsList:list<char>) (stack:list<char>) =
            match bracketsList with
            | h :: t -> 
                match h with
                | '(' | '[' | '{' -> checkBracketsWithStack t (h::stack)
                | ')' | ']' | '}' ->
                    if isPairBrackets stack.Head h then
                        checkBracketsWithStack t stack
                    else 
                        false
                | _ -> failwith "Incorrect brackets string"
            | [] -> 
                if stack.IsEmpty then
                    true
                else
                    false

        checkBracketsWithStack (Seq.toList bracketsString) []

