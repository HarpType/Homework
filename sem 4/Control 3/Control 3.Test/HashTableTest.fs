namespace Tests

module HashTableTest = 
    open NUnit.Framework
    open FsUnit
    open HashTable

    let testFunc key  = key % 256 |> byte

    [<Test>]
    let hashTableTest ()= 
        let testHashTable = HashTable(testFunc)

        testHashTable.Insert {Key = 58; Value = "HelloString"}
        testHashTable.Insert {Key = 1024; Value = "2^10"}
        testHashTable.Insert {Key = 578; Value = "A car washing"}
        
        testHashTable.Contains {Key = 1024; Value = "2^10"} |> should equal true
        testHashTable.Contains {Key = 578; Value = "A car washing"} |> should equal true

        testHashTable.Delete {Key = 578; Value = "A car washing"}

        testHashTable.Contains {Key = 578; Value = "A car washing"} |> should equal false

    [<Test>]
    let doubledItemTest ()=
        let testHashTable = HashTable(testFunc)

        testHashTable.Insert {Key = 58; Value = "HelloString"}
        testHashTable.Insert {Key = 58; Value = "HelloString"}

        testHashTable.Delete {Key = 58; Value = "HelloString"}

        testHashTable.Contains {Key = 58; Value = "HelloString"} |> should equal true
