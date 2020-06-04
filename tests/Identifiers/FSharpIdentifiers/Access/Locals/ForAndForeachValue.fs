module ForAndForeachValue

let func1 () =
  for iter = 1 to 10 do
    System.Console.WriteLine iter

let func2 () =
  for iter in [|5,6,8|] do
    System.Console.WriteLine iter