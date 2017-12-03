namespace FSharpHW

module MyList = 
    let reverse v = 
        let rec reverseHelper v acc = 
            match v with
            | [] -> acc
            | x :: xs -> reverseHelper xs (x :: acc)
        reverseHelper v []
