
using System.Net.Sockets;

User Adam = new User("Adam", "Adasek");


Server server = new Server();
server.Start();
while (true){
    if (server.Clients.Count == 2){

        if (server.Clients[0].IsDataAvailable){
            Model model = server.Clients[0].Data;
            Message message = (Message)model;
            server.Clients[0].Send(message);
            Message m2 = new Message(message.Data);
            server.Clients[1].Send(m2);
        } else if (server.Clients[1].IsDataAvailable){
            Model model = server.Clients[1].Data;
            Message message = (Message)model;
            server.Clients[1].Send(message);
            Message m2 = new Message(message.Data);
            server.Clients[0].Send(m2);
        }
        
        
    }
}
// Console.ReadLine();
// FullClient fullClient = new("127.0.0.1", 48025);

// fullClient.Send(Adam);

// Console.WriteLine(server.Clients.Count);
// await Task.Delay(4000);
// server.Stop();

