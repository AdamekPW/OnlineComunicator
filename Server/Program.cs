
using System.Net.Sockets;

User Adam = new User("Adam", "Adasek");


Server server = new Server();
server.Start();
while (true){
    if (server.Clients.Count == 1){
        while (!server.Clients[0].IsDataAvailable){};
        Model model = server.Clients[0].Data;
        if (model.type == typeof(Message)){
            Message message = (Message)model;
            message.Data += " - from server";
            server.Clients[0].Send(message);
        }
    }
}
// Console.ReadLine();
// FullClient fullClient = new("127.0.0.1", 48025);

// fullClient.Send(Adam);

// Console.WriteLine(server.Clients.Count);
// await Task.Delay(4000);
// server.Stop();

