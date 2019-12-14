module Attribute

open System.Runtime.CompilerServices

[<Extension>]
type Extension =
    [<System.Runtime.CompilerServices.Extension; MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member Send(arg:string) = 5