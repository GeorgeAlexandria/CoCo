module UnionCtorParameter

type Some =
    | Leaf of field : int

let f arg =
    match arg with
        | Leaf(field = 5) -> 0
        | _ -> 5