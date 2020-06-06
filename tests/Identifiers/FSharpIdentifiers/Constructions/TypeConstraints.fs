module TypeConstraints

type Some1<'T when 'T :> System.IDisposable> = class end
type Some2<'T when 'T : (member Value : unit -> int)> = class end
type Some3<'T when 'T : (member Value : int)> = class end
type Some4<'T when 'T : (static member Value : unit -> int)> = class end