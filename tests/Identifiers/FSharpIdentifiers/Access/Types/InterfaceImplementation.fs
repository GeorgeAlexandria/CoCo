module InterfaceImplementation

type ISome =
    abstract Get : unit -> string

type Some() =
    interface ISome with
        member __.Get () = ""