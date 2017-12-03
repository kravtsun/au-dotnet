namespace FSharpHW

open System

module Fib =
    let fib n = 
        if n < 0 then
            let message = sprintf "Invalid argument for fib: %d" n
            failwith message
        let rec fibHelper n = 
                if n = 0 then
                    (0, 1)
                else
                    let (prev, prevprev) = fibHelper (n - 1)
                    prevprev, prev + prevprev
        fst (fibHelper n)
            