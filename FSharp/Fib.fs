namespace FSharpHW

open NUnit.Framework
open FsUnit

module Fib =
    let fib n = 
        if n < 0 then
            let message = sprintf "Invalid argument for fib: %d" n
            failwith message
        let rec fibHelper n = 
                if n = 0 then
                    (0, 1)
                else
                    let (prevprev, prev) = fibHelper (n - 1)
                    prev, prevprev + prev
        fst (fibHelper n)

    [<Test>]
    let ``first 10 fibonacci numbers`` () =
        List.map fib [0..10] |> should equal [0; 1; 1; 2; 3; 5; 8; 13; 21; 34; 55]

    [<Test>]
    let ``negative number should fail`` () =
        (fun () -> fib -1 |> ignore) |> should throw typeof<System.Exception>
