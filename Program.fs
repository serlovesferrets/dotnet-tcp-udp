#nowarn "9"

open System
open System.Net
open System.Text
open System.Text.Json
open System.Net.Sockets

[<EntryPoint>]
let main _ =
    use server = new UdpClient 1100

    printfn "[S] Listening..."

    let endpoint = IPEndPoint(IPAddress.Loopback, 1200)

    let res
        : {| firstNumber: int
             secondNumber: int
             operation: string |} =
        server.Receive(ref endpoint) |> JsonSerializer.Deserialize

    server.Connect endpoint
    res |> printfn "[S] %A"

    let resp =
        match res.operation with
        | "+" -> res.firstNumber + res.secondNumber
        | "-" -> res.firstNumber - res.secondNumber
        | "*" -> res.firstNumber * res.secondNumber
        | "/" -> res.firstNumber / res.secondNumber
        | invalid ->
            $"Operazione invalida: \"{invalid}\""
            |> Encoding.ASCII.GetBytes
            |> server.Send
            |> ignore

            failwith "Invalid op received from client"


    let data = resp |> string |> Encoding.ASCII.GetBytes
    let _ = server.Send(data, data.Length)

    0
