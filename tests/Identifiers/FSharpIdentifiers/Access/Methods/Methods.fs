module Methods

type Some() =
    static member Get1() =
        5

    member __.Get2() =
        6

let res1 = Some.Get1()
let res2 = Some().Get2()
let res3 = Some.Get1
let res4 = Some().Get2