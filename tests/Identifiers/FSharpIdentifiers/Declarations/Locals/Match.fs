module Match

let ``func`` (arg:list<int>) =
    match arg with
        | head::tail -> 6
        | [] -> 0

let ``func2`` (arg:list<int>) =
    match (arg.Head, arg.Tail) with
        | (head, second::tail) -> 6
        | (last, []) -> 0

type Name =
    | First of string
    | Last of string

let some name =
    match name with
    | First first -> 0
    | Last last -> 5

let some2 name =
    match name with
    | First(first) -> 0
    | Last(last) -> 5