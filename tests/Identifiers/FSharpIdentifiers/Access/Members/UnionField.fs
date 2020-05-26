module UnionField

type Some =
    | Leaf of field : int

let get arg =  
    match arg with
        | Leaf (field = 5) -> ""