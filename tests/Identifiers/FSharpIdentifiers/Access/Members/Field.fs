module Field

type T =
    val value1 : int
    val mutable value2 : int

    member __.Get =
        __.value1 + __.value2