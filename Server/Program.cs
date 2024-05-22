
using System.Net.Sockets;

User Adam = new User("Adam", "Adasek");


Server server = new Server();
server.Start();
Console.ReadLine();
FullClient fullClient = new("127.0.0.1", 48025);
Console.ReadLine();
Console.WriteLine("Wysylanie");
//fullClient.SendASCII("Hello from ASCII");
fullClient.Send(Adam);
Console.WriteLine("Wyslano");

Console.ReadLine();
Console.WriteLine(server.Clients.Count);
server.Stop();

