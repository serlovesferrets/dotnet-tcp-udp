#nowarn "9"

open System
open System.Net
open System.Text
open System.Net.Sockets
open System.Text.Json

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

    use client = new UdpClient 1200
    client.Client.ReceiveTimeout <- 3000
    let serverEndpoint = new IPEndPoint(IPAddress.Loopback, 1100)

    let data =     
        {| firstNumber = int firstNumber
           secondNumber = int secondNumber
           operation = operation |}
        |> JsonSerializer.Serialize
        |> Encoding.ASCII.GetBytes

    printfn "[C] Sending data..."

    client.Connect serverEndpoint
    let _ = client.Send data

    printfn "[C] Data sent! Waiting for response..."

    let resp = client.Receive(ref serverEndpoint)
    resp |> Encoding.ASCII.GetString |> printfn "[C] %A"

main ()
