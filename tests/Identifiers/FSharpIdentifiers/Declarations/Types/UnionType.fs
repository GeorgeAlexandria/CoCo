module UnionType

type Tree =
    | Leaf of int
    | Node of Tree * Tree