#nowarn "9"

open System.Net
open System.Text
open System.Net.Sockets

let main () =
    use client = new UdpClient 1200
    client.Client.ReceiveTimeout <- 3000
    let serverEndpoint = new IPEndPoint(IPAddress.Loopback, 1100)

    let data = Encoding.ASCII.GetBytes "ping"

    printfn "[C] Sending data..."

    client.Connect serverEndpoint
    let _ = client.Send data

    printfn "[C] Data sent! Waiting for response..."

    let resp = client.Receive(ref serverEndpoint)
    resp |> Encoding.ASCII.GetString |> printfn "[C] %A"

main ()
