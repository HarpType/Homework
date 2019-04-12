module StarsPrint

// Тип определяет варианты создания одной строки из звёздочек.
type Option =
    | RowOption
    | ColumnOption


// Функция формирует строку, в которой находится квадрат из звёздочек размера n на n.
let squareStarString starsNumber =
    // Создаёт одну строчку из квадрата звёздочек. 
    // Если текущая опция - RowOption, то возвращает строку из n звёздочек.
    // Если текущая опция - ColumnOption, то возвращает строку из n символов 
    // вида *      *.
    let rec createStarString n i option starsString= 
        if i = n+1 then
            starsString + "\n"
        else
            match option with
            | RowOption -> 
                createStarString n (i+1) option (starsString + "*")
            | ColumnOption ->
                if i = 1 || i = n then
                    createStarString n (i+1) option (starsString + "*")
                else 
                    createStarString n (i+1) option (starsString + " ")
    
    // Собирает n строк квадрата в одну строку.
    let rec starStringAccum n i accumString = 
        if i = n + 1 then
            accumString
        elif i = 1  || i = n then
            starStringAccum n (i+1) (accumString + createStarString n 1 RowOption "")
        else starStringAccum n (i+1) (accumString + createStarString n 1 ColumnOption "")

    starStringAccum starsNumber 1 ""


