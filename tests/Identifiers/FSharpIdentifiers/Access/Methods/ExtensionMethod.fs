module ExtensionMethod
open IntrinsicExtensionMethod
open OptionalExtensionMethod
open System.Runtime.CompilerServices

[<Extension>]
type Extension =
    [<Extension>]
    static member Get3(arg:Some) = 5

let z = Some()

let res1 = z.Get1()
let res2 = z.Get2()
let res3 = z.Get3()