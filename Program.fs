#nowarn "9"

open System
open System.Net
open System.Text
open System.Net.Sockets

[<EntryPoint>]
let main _ =
    use server = new UdpClient 1100

    printfn "[S] Listening..."

    let endpoint = IPEndPoint(IPAddress.Loopback, 1200)
    let res = server.Receive(ref endpoint)
    server.Connect endpoint

    res |> Encoding.ASCII.GetString |> printfn "[S] %A"

    let data = Encoding.ASCII.GetBytes "pong"
    let _ = server.Send(data, data.Length)

    0
