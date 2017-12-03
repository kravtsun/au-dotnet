namespace FSharpHW

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
                | (x1 :: xs1) -> match v2 with
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
            

