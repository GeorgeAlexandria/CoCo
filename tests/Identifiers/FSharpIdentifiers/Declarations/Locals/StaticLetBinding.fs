module StaticLetBinding

type Some() =
    static let first : int = 5
    static let mutable second : int = 5 * 2