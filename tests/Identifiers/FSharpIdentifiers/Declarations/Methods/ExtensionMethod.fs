module ExtensionMethod

open IntrinsicExtensionMethod
open System.Runtime.CompilerServices

[<Extension>]
type Extension =
    [<Extension>]
    static member Send(arg:Some) = 5