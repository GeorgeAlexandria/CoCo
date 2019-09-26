module ModuleFunctionParams

let ``fun`` arg =
    arg * 2

let ``fun typed`` (arg : int) : int =
        arg * 2