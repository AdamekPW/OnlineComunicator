using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
class Server {
    public string ServerIP {get; set;}
    public int ServerPort {get; set;}

    public List<string> ActiveUsers = new();
    Thread ServerThread = null!;
    Process? TunnelProcess = null;
    
    private bool IsServerRunning = false;

    public Server(){
        this.ServerIP = "127.0.0.1";
        this.ServerPort = 48025;
    }
    public Server(string ServerIP, int ServerPort){
        this.ServerIP = ServerIP;
        this.ServerPort = ServerPort;
    }

    ~Server() {
        Stop();
        KillTunnel();
        Console.WriteLine("Server is offline");
    }
    
    public void Start(){
        if (IsServerRunning){
            Console.WriteLine("Server already running!\n");
            return;
        }
        ServerThread = new Thread(this.Run);
        ServerThread.Start();
        IsServerRunning = true;
        Console.WriteLine("Server started");
    }
    public void Stop(){
        if (!IsServerRunning){
            Console.WriteLine("Server is not running");
            return;
        }
        IsServerRunning = false;
        Console.WriteLine("Server stoped");       
    }
    

    private void Run(){
        TcpListener server = null!;
        IPAddress iPAddress = IPAddress.Parse(ServerIP);
        try 
        {
            server = new TcpListener(iPAddress, ServerPort);
            server.Start();
            Console.WriteLine("Serwer TCP uruchomiony na adresie: " + ServerIP + " na porcie: " + ServerPort);
			Console.WriteLine("Oczekiwanie na połączenia...");

            while (IsServerRunning){
                if (server.Pending()){
                    

                    TcpClient client = server.AcceptTcpClient();
                    Console.WriteLine("Nowe połączenie!");
                    
                    Model? model = HandleClient(client);
                    if (model != null){
                        Console.WriteLine(model.type + " " + model.GetType());
                        if (model.GetType() == typeof(User)){
                            User m = (User)model;
                            Console.WriteLine(m.Username);
                        }
                    }

                    client.Close();
                
                   
                    
                }     
				
            }
        }
        catch (Exception e){
            Console.WriteLine("Błąd: " + e.Message);
        }
        finally {
            server.Stop();
        }
    }

    public void StartTunnel(){
        if (TunnelProcess != null){
            Console.WriteLine("Tunnel already running");
            return;
        }
        TunnelProcess = new Process();
        TunnelProcess.StartInfo.FileName = @"C:\Program Files\playit_gg\bin\playit.exe";
        TunnelProcess.Start();
        
    }

    public void KillTunnel(){
        if (TunnelProcess == null){
            Console.WriteLine("Tunnel doesn't exist");
            return;
        }
        TunnelProcess.Kill();
        TunnelProcess = null;
        Console.WriteLine("Tunnel terminated");
    }


    public Model? HandleClient(TcpClient client)
	{
		Model? model = null;
		NetworkStream stream = null!;
		try
		{
			stream = client.GetStream();

			byte[] buffer = new byte[1024];
			int bytesRead;
			string JsonString = "";
			
			while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) > 0)
			{
				JsonString += Encoding.UTF8.GetString(buffer, 0, bytesRead);
				if (bytesRead < buffer.Length)
				{
					break;
				}
			}
			dynamic? m = JsonConvert.DeserializeObject(JsonString);
            if (m != null){
                Type type = (Type)m["type"];
                switch (type.ToString()){
                    case "User": 
                        model = JsonConvert.DeserializeObject<User>(JsonString);;
                        break;
                    case "Message":
                        Message message = JsonConvert.DeserializeObject<Message>(JsonString);
                        Console.WriteLine(message.Data);
                        break;
                    default: 
                        model = null;
                        break;
                }
            }
               
            
		}
		catch (Exception e)
		{
			Console.WriteLine("Błąd obsługi klienta: " + e.Message);
		}
		finally
		{
			stream.Close();
			client.Close();
		}
		return model;
	}
}