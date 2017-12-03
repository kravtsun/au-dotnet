namespace FSharpHW
open Fib
open MyList
module Program =
    [<EntryPoint>]
    let main argv = 
//        let testFib = List.map (fun i -> printfn "fib(%d) = %d" i (fib i)) [0..10]
//        let testReverse = 
//            let v = [1..3]
//            printfn "v = \t%A" v
//            printfn "rev = \t%A" (reverse v)
        let testSort = 
            let singleTest v = 
                printfn "%A -> %A" v (sort v)
            singleTest [1]
            singleTest [1; 2]
            singleTest [2; 1]
            singleTest [1..3]
            singleTest [3; 2; 1]
            let randomTest count maxNumber = 
                let rnd = System.Random()
                let randomNumbers = List.init count (fun _ -> rnd.Next(maxNumber))
                singleTest randomNumbers
            randomTest 100 100
        0
