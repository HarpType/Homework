open System
open PhoneBook

[<EntryPoint>]
let main argv =
    stepPhoneBook EmptyNode |> ignore
    0 // return an integer exit code
