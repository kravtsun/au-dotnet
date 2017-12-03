namespace FSharpHW
open Fib
open MyList
module Program =
    [<EntryPoint>]
    let main argv = 
        let testFib = List.map (fun i -> printfn "fib(%d) = %d" i (fib i)) [0..10]
        let testReverse = 
            let v = [1..3]
            printfn "v = \t%A" v
            printfn "rev = \t%A" (reverse v)
        0
