
User Adam = new User("Adam", "Adasek");


Server server = new Server();
server.Start();
Console.ReadLine();
CustomClient client = new CustomClient();
client.Login(Adam);


Console.WriteLine(server.Clients.Count);
server.Stop();

