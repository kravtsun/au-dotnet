namespace FSharpHW
open Fib
module Program =
    [<EntryPoint>]
    let main argv = 
    //    printfn "%A" argv
        let fib1 = fib 1
        let result = List.map (fun i -> printfn "fib(%d) = %d" i (fib i)) [0..10]
        printfn "%d" fib1
        0 // return an integer exit code
