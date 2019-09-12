module RecursiveClassType

type Some() =
    let z = new Some2()
and Some2() =
    let z = new Some()