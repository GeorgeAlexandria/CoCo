module Properties

type PropertyWithGet_Set =
    member __.Value
        with get() =
               5
        and set (arg:int) =
           ()

type PropertyWithGet =
    member __.Value
        with get() =
            5

type PropertyWithSet =
    member __.Value
        with set arg =
            ()

type ReadonlyProperty =
    member __.Value =
            5

type PropertyWithGet_SetAlternative =
    member __.Value =
        4
    member __.Value
        with set (arg:int) =
            ()

type AutoProperty() =
    member val Value = 5 + 5 with get, set