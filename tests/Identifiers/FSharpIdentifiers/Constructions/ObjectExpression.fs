module ObjectExpression

type Some(arg1:int, arg2:int) = class end

let value = { new Some(5, arg2 = 9) with member this.ToString() = "F#" }
