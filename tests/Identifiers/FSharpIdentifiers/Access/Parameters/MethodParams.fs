module MethodParams

type Some =
    member __.Get arg =
        let value = arg
        let arg = 5
        ()