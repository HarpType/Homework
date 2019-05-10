namespace Tests

module HashTableTest = 
    open NUnit.Framework
    open FsUnit
    open HashTable

    let testFunc key  = key % 256 |> byte

    [<Test>]
    let hashTableTest ()= 
        let testHashTable = HashTable(testFunc)

        testHashTable.insert {Key = 58; Value = "HelloString"}
        testHashTable.insert {Key = 1024; Value = "2^10"}
        testHashTable.insert {Key = 578; Value = "A car washing"}
        
        testHashTable.contains {Key = 1024; Value = "2^10"} |> should equal true
        testHashTable.contains {Key = 578; Value = "A car washing"} |> should equal true

        testHashTable.delete {Key = 578; Value = "A car washing"}

        testHashTable.contains {Key = 578; Value = "A car washing"} |> should equal false
