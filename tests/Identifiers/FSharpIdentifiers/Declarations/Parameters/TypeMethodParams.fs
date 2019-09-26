module TypeMethodParams

type Some() =
    member __.``fun`` arg = 
        arg * 2

    member __.``fun typed`` (arg : int) : int =
        arg * 2