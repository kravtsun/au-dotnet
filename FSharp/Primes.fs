namespace FSharpHW

open NUnit.Framework
open FsUnit

module Primes = 
    // natural series starting from 2.
    let naturalSeries2 = Seq.initInfinite (fun x -> x + 2)

    let infinite = 
        let isPrime x = Seq.forall (fun y -> x % y <> 0) (Seq.takeWhile (fun y -> y < x) naturalSeries2)
        Seq.filter isPrime naturalSeries2

    [<Test>]
    let ``first 10 prime numbers should be correct`` () =
        let truncated = infinite |> Seq.truncate 10 |> Seq.toList
        truncated |> should equal [2; 3; 5; 7; 11; 13; 17; 19; 23; 29]

    [<Test>]
    let ``empty truncation should finish`` () =
        let truncated = infinite |> Seq.truncate 0 |> Seq.toList
        truncated |> should equal []
