module UseBinding

let some =
    use file = System.IO.File.CreateText("")
    file.WriteLine("")
    ()