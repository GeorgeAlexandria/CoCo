module FunctionParams

let funny arg =
    let value = arg
    let arg = 5
    ()

let funny2 (arg:int) : int =
    let value = arg
    5

let rec recFun (arg:int) : unit =
    let value = arg
    ()