#nowarn "9"

open System.Text
open System.Net.Sockets
open System
open Microsoft.FSharp.NativeInterop

let main () =
    let args = fsi.CommandLineArgs

    if Array.length args = 1 then
        failwith "[C] Il messaggio non può essere vuoto!"

    let msg = StringBuilder().AppendJoin(" ", args[1..]) |> string
    let bytes = Encoding.ASCII.GetBytes msg

    printfn $"[C] Sending: <{msg}>"

    let server = new TcpClient("127.0.0.1", 5005)
    let stream = server.GetStream()

    stream.Write bytes

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
