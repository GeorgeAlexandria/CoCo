module RecordFieldInPatternCase

type T =
    {
        field : string
    }

    member __.Get arg =
        match arg with
            | Some {field="val"} -> 5
            | None -> 0