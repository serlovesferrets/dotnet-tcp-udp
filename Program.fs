#nowarn "9"

open System.Net
open System.Net.Sockets
open System.Text
open System.Text.Json

[<Struct>]
type Operation =
    | Sum
    | Sub
    | Prod
    | Div
    | Invalid

    static member fromStr =
        function
        | "+" -> Sum
        | "-" -> Sub
        | "*" -> Prod
        | "/" -> Div
        | _ -> Invalid

[<Struct>]
type Request =
    { firstNumber: int
      secondNumber: int
      operation: string }


[<EntryPoint>]
let main _ =
    let ip = IPAddress.Parse "127.0.0.1"
    use server = new TcpListener(ip, 5005)

    printfn "[S] Calcolatrice, %A" server.LocalEndpoint
    server.Start()

    while true do
        use client = server.AcceptTcpClient()
        use clientStream = client.GetStream()

        let mutable respBytes = Array.zeroCreate 256

        let _ = clientStream.Read(respBytes, 0, respBytes.Length)

        let msg =
            Encoding.ASCII.GetString respBytes //
            |> Seq.filter (fun t -> t <> '\000')
            |> Seq.toArray

        let data = JsonSerializer.Deserialize<Request> msg

        let result =
            match Operation.fromStr data.operation with
            | Invalid -> "Operazione invalida!"
            | valid ->
                (match valid with
                 | Sum -> (+)
                 | Sub -> (-)
                 | Prod -> (*)
                 | Div -> (/)
                 | _ -> failwith "Impossible case")
                |> fun op -> op data.firstNumber data.secondNumber
                |> string

        $"[S] Risultato: {result}" //
        |> Encoding.ASCII.GetBytes
        |> clientStream.Write


    server.Stop()

    0
