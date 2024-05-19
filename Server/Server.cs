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
                    bool IsRead = false;
                    TcpClient _client = server.AcceptTcpClient();   
                    Console.WriteLine("Client Task started");
                    Task.Run(() => {
                        TcpClient client = _client;
                        IsRead = true;
                        Console.WriteLine("Nowe połączenie!");
                        FullClient fullClient = new FullClient(client);

                        //logowanie
                        Model? model = HandleClient(client);
                        if (model == null || model.GetType() != typeof(User)) return;

                        User user = (User)model;
                        Console.WriteLine($"Proba logowania uzytkownika {user.Username}");
        
                        if (!Login(user)){
                            fullClient.SendASCII("Zle haslo");
                            return;
                        } 
                        fullClient.SendASCII("Zalogowano pomyslnie");
                        
                        fullClient.user = user;
                        fullClient.Run();
                        
                        Clients.Add(fullClient);
                    });
                    while (!IsRead){};
                    
                    

                    
                }     
				
            }
            
        }
        catch (Exception e){
            Console.WriteLine("Błąd: " + e.Message);
        }
        finally {
            server.Stop();
            Clients.Clear();
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