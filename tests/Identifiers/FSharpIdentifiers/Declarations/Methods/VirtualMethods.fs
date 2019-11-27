module VirtualMethods

[<AbstractClass>]
type Some() =
    abstract member get: int -> int

type Some2() =
    abstract member get:int-> int
    default __.get (arg:int) = 5