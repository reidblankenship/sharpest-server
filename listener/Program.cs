namespace listener;
using System.Net;
using System.Net.Sockets;
using System.Text;

class Listener
{
    static void Main()
    {
        TcpServer server = new TcpServer();
    }
}

class TcpServer
{

    private NetworkStream? stream;
    private TcpListener listener;
    private TcpClient? client;
    private bool connected;


    public TcpServer()
    {

        listener = new TcpListener(IPAddress.Parse("127.0.0.1"), 8888);
        try
        {

            listener.Start();
            Console.WriteLine("Server listening on 127.0.0.1:8888....");
            client = listener.AcceptTcpClient();
        }
        catch (SocketException se)
        {
            Console.Error.WriteLine(se.Message);
            Console.Error.WriteLine("Socket Error Code = " + se.ErrorCode);
            return;
        }
        Console.WriteLine("Client Connected.");
        Console.WriteLine("Client Receive Timeout = " + client.ReceiveTimeout);
        Console.WriteLine("Client Send Timeout = " + client.SendTimeout);

        stream = client.GetStream();

        connected = true;
        while (connected)
        {
            string response = GetMessage();
            Console.WriteLine($"Received: {response}");
            if (response == "Stop.")
            {
                break;
            }
            Respond(ProcessMessage(response));
        }
        Stop();
    }

    public void Stop()
    {
        stream?.Close();
        client?.Close();
        listener.Stop();
    }

    string GetMessage()
    {
        if (stream == null)
        {
            connected = false;
            return "Stop.";
        }
        byte[] buffer = new byte[1024];
        int bytesRead = -1;
        try
        {
            bytesRead = stream.Read(buffer, 0, buffer.Length);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine(e.Message);
            Console.Error.WriteLine(e.StackTrace);
        }
        if (bytesRead == -1)
        {
            Console.Error.WriteLine("No bytes were read from the NetworkStream.");
            throw new Exception();
        }
        string receivedData = Encoding.UTF8.GetString(buffer, 0, bytesRead);
        return receivedData;
    }

    void Respond(string response)
    {
        if (stream == null)
        {
            connected = false;
            return;
        }
        byte[] responseData = Encoding.UTF8.GetBytes(response);
        stream.Write(responseData, 0, responseData.Length);
    }

    static string ProcessMessage(string received)
    {
        if (received == null)
        {
            return "Foo, Bar.";
        }
        string result = "";
        switch (received)
        {
            case "Stop.":
                result = "We understand. Have a nice day.";
                break;
            case "Hello.":
                result = "Hi, how are you?";
                break;

            case "Goodbye.":
                result = "Fun talking with you.";
                break;
            default:
                result = reverse(received);
                break;
        }
        return result;
    }

    static string reverse(string input)
    {
        return new string(input.ToCharArray().Reverse().ToArray());
    }
}
