using System;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Newtonsoft.Json;
public partial class Server  {
    public string ServerIP {get; set;}
    public int ServerPort {get; set;}

    public List<FullClient> Clients = new(); 



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
                    
                    
                    TcpClient testClient = server.AcceptTcpClient();   
                    Console.WriteLine("Nowe połączenie!");
                    FullClient fullClient = new FullClient(testClient);

                    //logowanie
                    Model? model = HandleClient(testClient);
                    if (model == null || model.GetType() != typeof(User)) continue;

                   
                    User user = (User)model;
                    Console.WriteLine($"Proba logowania uzytkownika {user.Username}");
                    string res = Login(user) == true ? "Zalogowano pomyslnie" : "Zle haslo"; 
                    
                    byte[] msg = Encoding.ASCII.GetBytes(res);
                    var stream = testClient.GetStream();    
                    stream.Write(msg);
                    
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


    
}