#nowarn "9"

open System.Net
open System.Net.Sockets
open System.Text

[<EntryPoint>]
let main _ =
    let ip = IPAddress.Parse "127.0.0.1"
    use server = new TcpListener(ip, 5005)

    printfn "[S] %A" server.LocalEndpoint
    server.Start()

    use client = server.AcceptTcpClient()
    use clientStream = client.GetStream()

    let mutable respBytes = Array.zeroCreate 256

    let _ = clientStream.Read(respBytes, 0, respBytes.Length)

    let msg = Encoding.ASCII.GetString respBytes
    printfn $"[S] Received: <{msg}>"

    "[S] Messaggio ricevuto dal server" //
    |> Encoding.ASCII.GetBytes
    |> clientStream.Write

    server.Stop()

    0
