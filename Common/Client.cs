using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
public class CustomClient : NetworkStreamManager
{
    public string ServerIP { get; set; } = null!;
	public int ServerPort { get; set; }

    public TcpClient tcpClient;
    
    //147.185.221.19:48025
    public CustomClient() {
        this.ServerIP = "127.0.0.1";
        this.ServerPort = 48025;
        tcpClient = new TcpClient(ServerIP, ServerPort);
        stream = tcpClient.GetStream();
    }
	public CustomClient(string ServerIP, int ServerPort)
	{
		this.ServerIP = ServerIP;
		this.ServerPort = ServerPort;
        tcpClient = new TcpClient(ServerIP, ServerPort);
        stream = tcpClient.GetStream();
	}


    public void Login(User user)
    {
        if (user.Username.Length < 1 || user.Password.Length < 1)
            return;
        Send(user);
        string res = ReadASCII();
        Console.WriteLine(res);
        ReadAndShow();
    }

    public void ReadAndShow(){
        while (true){
            if (stream.CanRead && stream.DataAvailable){
					
                Model? model = Read();
                if (model == null) continue;
                Model Data = model;
                if (model.type == typeof(Message)){
                    Message m = (Message)model;
                    Console.WriteLine(m.Data);
                }
            } 
        }
    }
   
    



}