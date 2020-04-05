module TypeParameterType

type Some<'TValue when 'TValue: null>() = 
    member __.Get() : 'TValue =
        null