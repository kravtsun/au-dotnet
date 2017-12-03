namespace FSharpHW

open NUnit.Framework
open FsUnit

module MyList = 
    let reverse v = 
        let rec reverseHelper v acc = 
            match v with
            | [] -> acc
            | x :: xs -> reverseHelper xs (x :: acc)
        reverseHelper v []

    let rec sort v = 
        match v with
        | [] -> v
        | [x] -> v
        | _ -> 
            let split v = 
                match v with
                | [x] -> (v, [])
                | _ -> 
                    let rec splitHelper (first: int list) firstLength second secondLength = 
                        if firstLength >= secondLength then
                            (first, second)
                        else
                            match second with
                            | (x :: xs) -> splitHelper (x :: first) (firstLength + 1) xs (secondLength - 1)
                            | _ -> failwith "Unexpected branch in split"
                    splitHelper [] 0 v (List.length v)
            let rec merge v1 v2 =
                match v1 with
                | [] -> v2
                | (x1 :: xs1) ->
                    match v2 with
                    | [] -> v1
                    | (x2 :: xs2) ->
                        if x1 < x2 then
                            x1 :: merge xs1 v2
                        else if x2 < x1 then
                            x2 :: merge v1 xs2
                        else
                            [x1; x2] @ (merge xs1 xs2)
            let (v1, v2) = split v
            merge (sort v1) (sort v2)

    [<Test>]
    let ``sort on empty should return empty`` () =
        sort [] |> should equal []

    [<Test>]
    let ``sort on sorted sequence should return the same`` () =
        let v = [1; 2; 3]
        sort v |> should equal v

    [<Test>]
    let ``sort on reversed sequnce should return sorted`` () =
        let v = [3; 2; 1]
        let rev = [1; 2; 3]
        sort v |> should equal rev

    let bigRandomList count maxnumber =
        let rnd = System.Random()
        List.init count (fun _ -> rnd.Next(maxnumber))

    [<Test>]
    let ``sort on big random list should work`` () =
        let v = bigRandomList 100 100
        sort v |> should be ascending

    [<Test>]
    let ``reverse on empty should return empty`` () =
        reverse [] |> should equal []

    [<Test>]
    let ``reverse on simple sequence should work`` () =
        reverse [1; 2; 3] |> should equal [3; 2; 1]

    [<Test>]
    let ``reverse on big random list should work`` () =
        let v = bigRandomList 100 100
        let vsorted = sort v
        reverse vsorted |> should be descending
