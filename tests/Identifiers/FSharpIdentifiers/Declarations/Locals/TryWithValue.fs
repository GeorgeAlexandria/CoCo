module TryWithValue

let ``function`` () =
  try
    0/0
  with
    | :? System.ArgumentException as ex1 -> 5
    | _ -> 0