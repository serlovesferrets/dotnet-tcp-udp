#nowarn "9"

open System.Text
open System.Text.Json
open System.Net.Sockets
open System
open Microsoft.FSharp.NativeInterop

let ask prompt =
    printf "%A\n> " prompt
    Console.ReadLine() |> fun s -> s.Trim()

let main () =
    let firstNumber = ask "Inserisci il primo numero"

    if firstNumber |> Int32.TryParse |> fst |> not then
        failwith "Non un numero!"

    let secondNumber = ask "Inserisci il secondo numbero"

    if secondNumber |> Int32.TryParse |> fst |> not then
        failwith "Non un numero!"

    let operation = ask "Inserisci un'operazione (+, -, *, /)"

    let server = new TcpClient("127.0.0.1", 5005)
    let stream = server.GetStream()

    {| firstNumber = int firstNumber
       secondNumber = int secondNumber
       operation = operation |}
    |> JsonSerializer.Serialize
    |> Encoding.ASCII.GetBytes
    |> stream.Write

    printfn "[C] Bytes written!"

    let mem = NativePtr.stackalloc<byte> (256 * 4) |> NativePtr.toVoidPtr
    let mutable span = Span<byte>(mem, 256 * 4)

    let mutable shouldReceive = true

    while shouldReceive do
        if stream.Read span = 0 then
            shouldReceive <- false

    let response = Encoding.ASCII.GetString span

    printfn $"[C] Server resp: <{response}>"


main ()
