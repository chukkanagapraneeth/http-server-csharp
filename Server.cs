using System.Net;
using System.Net.Sockets;
using System.Text;

// You can use print statements as follows for debugging, they'll be visible when running tests.
Console.WriteLine("Logs from your program will appear here!");

// Uncomment this block to pass the first stage
TcpListener server = new TcpListener(IPAddress.Any, 4221);
server.Start();


while (true)
{
    Socket socket = server.AcceptSocket();

    var buffer = new byte[1_024];
    var req = socket.Receive(buffer);

    var reqStr = Encoding.UTF8.GetString(buffer, 0, req);

    string reqUrl = reqStr.Split(' ')[1];
    string[] reqUrlParts = reqUrl.Split('/');

    if (reqUrl == "/")
    {
        socket.Send(Encoding.UTF8.GetBytes("HTTP/1.1 200 OK\r\n\r\n"));
    }
    else if (reqUrlParts[1] == "echo")
    {
        if (reqUrlParts.Length > 2)
        {
            string textToSend = reqUrlParts[2];
            socket.Send(Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {textToSend.Length}\r\n\r\n{textToSend}"));
        }
        else
        {
            socket.Send(Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: 0\r\n\r\n"));
        }
    }
    else if (reqUrl == "/user-agent")
    {
        string responseToSend = reqStr.Split("\r\n")[1].Split(":")[1].Trim();
        socket.Send(Encoding.UTF8.GetBytes($"HTTP/1.1 200 OK\r\nContent-Type: text/plain\r\nContent-Length: {responseToSend.Length}\r\n\r\n{responseToSend}"));
    }
    else
    {
        socket.Send(Encoding.UTF8.GetBytes("HTTP/1.1 404 Not Found\r\n\r\n"));
    }

    socket.Close();
}