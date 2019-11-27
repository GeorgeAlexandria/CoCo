module IndexedProperties

type PropertyWithGet_Set =
    member __.Value
        with get(arg:int) =
               5
        and set (arg0:int)(arg:int) =
           ()

type PropertyWithGet =
    member __.Value
        with get(arg:int) =
            5

type PropertyWithSet =
    member __.Value
        with set arg0 arg =
            ()