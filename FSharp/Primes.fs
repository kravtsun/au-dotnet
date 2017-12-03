namespace FSharpHW

module Primes = 
    // natural series starting from 2.
    let naturalSeries2 = Seq.initInfinite (fun x -> x + 2)

    let infinite = 
        let isPrime x = Seq.forall (fun y -> x % y <> 0) (Seq.takeWhile (fun y -> y < x) naturalSeries2)
        Seq.filter isPrime naturalSeries2
