module AutoPropertyWithImplicitCtor

type T() =
    member val value = 6

    member __.Get =
        __.value