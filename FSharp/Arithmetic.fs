namespace FSharpHW

open NUnit.Framework
open FsUnit

module Arithmetic =
    type Expression =
        | Const of int
        | Add of Expression * Expression
        | Sub of Expression * Expression
        | Mul of Expression * Expression
        | Div of Expression * Expression

    let rec eval (e: Expression) =
        match e with
        | Const x -> x
        | Mul(x, y) -> (eval x) * (eval y)
        | Div(x, y) -> (eval x) / (eval y)
        | Add(x, y) -> (eval x) + (eval y)
        | Sub(x, y) -> (eval x) - (eval y)

    [<Test>]
    let ``add should work`` () =
        Add(Const 2, Add(Const 3, Const 4)) |> eval |> should equal 9

    [<Test>]
    let ``subtraction should work`` () =
        Sub(Const 1, Const 2) |> eval |> should equal -1

    [<Test>]
    let ``multiplication should work`` () =
        Mul(Const 2, Const 2) |> eval |> should equal 4

    [<Test>]
    let ``division should work`` () =
        Div(Const 8, Const 3) |> eval |> should equal 2
