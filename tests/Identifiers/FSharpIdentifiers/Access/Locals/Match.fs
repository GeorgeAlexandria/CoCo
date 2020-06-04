module Match

let ``func`` (arg:list<int>) =
    match arg with
        | head::tail -> head * List.head tail
        | [] -> 0

let ``func2`` (arg:list<int>) =
    match (arg.Head, arg.Tail) with
        | (head, second::tail) -> head * second
        | (last, []) -> 0

type Name =
    | First of string
    | Last of string

let some name =
    match name with
    | First first -> first
    | Last last -> last

let some2 name =
    match name with
    | First(first) -> first
    | Last(last) -> last

let ``func3`` (arg:list<int>) =
    match arg with
        | [some1; some2] -> some1 + some2
        | any -> 5