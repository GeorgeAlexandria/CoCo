module UnionCases

type Fruit =
    | Apple
    | Banana

let func1 arg =
    match arg with
        | Fruit.Apple -> 5
        | Fruit.Banana -> 6

let func2 arg =
    match arg with
        | 0 -> Apple
        | _-> Banana