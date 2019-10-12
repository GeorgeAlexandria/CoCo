module ImplicitCtorParamsShadowing

type Some(arg:int, something:string) as __=
    let value = arg
    do
        let inner = arg
        let arg, something = 5, "asd"
        let newValue = arg
        ()