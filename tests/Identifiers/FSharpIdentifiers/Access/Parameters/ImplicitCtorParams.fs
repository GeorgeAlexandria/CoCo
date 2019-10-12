module ImplicitCtorParams

type Some(arg:int) as __=
    let value = arg
    do
        let inner = arg
        ()