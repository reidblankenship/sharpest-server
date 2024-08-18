using System.Net;
using System.Net.Sockets;
using System.Text;


namespace client;

class Client
{
    public static void Main()
    {
        RunningTcpClient runner = new RunningTcpClient();
        if (!runner.Startable)
        {
            Console.Error.WriteLine("Client wasn't startable. Terminating...");
            return;
        }
        Console.WriteLine("Start conversation? (n/Y)");
        var input = Console.ReadLine();
        if (input == "Y" || input == "y")
        {
            runner.MaintainConnection();
        }
        else
        {
            runner.Stop();
        }
    }
}

class RunningTcpClient
{
    private TcpClient client;
    private NetworkStream? stream;
    public bool Startable = false;
    public RunningTcpClient()
    {
        client = new TcpClient();
        try
        {
            client.Connect(IPAddress.Parse("127.0.0.1"), 8888);
            stream = client.GetStream();
        }
        catch (ObjectDisposedException ode)
        {
            Console.WriteLine("TCP Server not found. (Are you sure you're typing it in correctly?)");
            Console.Error.WriteLine(ode.Message);
        }
        catch (SocketException se)
        {
            Console.WriteLine("Socket Error.");
            Console.Error.WriteLine(se.Message);
        }

        if (client.Connected)
        {
            Console.WriteLine("Connected to server at: 127.0.0.1:8888");
            Startable = true;
        }
    }


    public void MaintainConnection()
    {
        while (client.Connected)
        {
            try
            {
                string? message = Console.ReadLine() ?? "Stop.";
                string messageReceived = SendAndReceive(message);
                Console.WriteLine($"Message from Server: {messageReceived}");
                if (message == "Stop.")
                {
                    break;
                }
            }
            catch (ThreadInterruptedException tie)
            {
                Console.Error.WriteLine(tie.Message);
                break;
            }
        }
        Stop();
    }

    public void Stop()
    {
        stream?.Close();
        client.Close();
    }

    private string SendAndReceive(string sent)
    {
        byte[] sentData = Encoding.UTF8.GetBytes(sent);
        stream?.Write(sentData, 0, sentData.Length);

        byte[] receivedData = new byte[1024];
        int bytesRead = stream?.Read(receivedData, 0, receivedData.Length) ?? -1;
        if (bytesRead == -1)
        {
            Console.Error.WriteLine("Error occurred with NetworkStream. Terminating...");
            throw new Exception();
        }

        string received = Encoding.UTF8.GetString(receivedData, 0, bytesRead);
        return received;
    }
}
