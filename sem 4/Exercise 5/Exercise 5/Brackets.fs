module Brackets
    
    /// Возвращает функцию, которая проверяет корректность скобочной последовательности
    /// введённой строки. Типы скобок задаются вручную.
    /// oBri -- открывающая скобка типа i.
    /// сBri -- закрывающая скобка типа i.
    let bracketsStringChekerFactory (oBr1, cBr1) (oBr2, cBr2) (oBr3:char, cBr3) =

        if (oBr1 = cBr1) || (oBr2 = cBr2) || (oBr3 = cBr3) then
            failwith "Opening and closing brackets must not be equal"
        
        /// Возвращает true, если открывающаяся и закрывающаяся скобки
        /// одного типа. false в противном случае.
        let isPairBrackets openingBracket closingBracket =
            if (openingBracket = oBr1) && (closingBracket = cBr1) then
                true
            elif (openingBracket = oBr2) && (closingBracket = cBr2) then
                true
            elif (openingBracket = oBr3) && (closingBracket = cBr3) then
                true
            else false

        /// Возвращает true, если строка скобок корректна и false в противном случае.
        /// Вызывает исключение, если в строке присутствуют посторонние символы.
        let isCorrectBracketsString bracketsString =
            let rec checkBracketsWithStack (bracketsList:list<char>) (stack:list<char>) =
                match bracketsList with
                | h :: t -> 
                    if (h = oBr1) || (h = oBr2) || (h = oBr3) then
                        checkBracketsWithStack t (h::stack)
                    elif ((h = cBr1) || (h = cBr2) || (h = cBr3)) then
                        if not stack.IsEmpty then
                            if isPairBrackets stack.Head h then
                                checkBracketsWithStack t stack.Tail
                            else 
                                false
                        else 
                            false
                    else 
                        checkBracketsWithStack t stack
                | [] -> 
                    if stack.IsEmpty then
                        true
                    else
                        false

            checkBracketsWithStack (Seq.toList bracketsString) []

        isCorrectBracketsString
