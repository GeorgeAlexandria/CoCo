module Property

type PropertyWithGet_Set =
    member __.Value
        with get() =
               5
        and set (arg:int) =
           ()

let o = new PropertyWithGet_Set()
let p = o.Value 
let b = o.Value <- 5