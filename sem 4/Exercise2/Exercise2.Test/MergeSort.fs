namespace Tests

module MergeSort =
    open NUnit.Framework
    open FsUnit
    open Main

    [<Test>]
    let emptySorting ()=
        mergeSort [] |> should equal []

    [<Test>]
    let ``List should be [1; 2; 3; 4; 5]`` ()=
        mergeSort [2; 5; 3; 4; 1] |> should equal [1; 2; 3; 4; 5]

    [<Test>]
    let charsSorting ()=
        mergeSort ['v'; 'k'; 'q'; 'a'; 'k'; 'b'; 'p'; 'w'] |> 
            should equal ['a'; 'b'; 'k'; 'k'; 'p'; 'q'; 'v'; 'w']
