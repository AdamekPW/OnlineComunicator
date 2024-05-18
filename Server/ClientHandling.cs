using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

public class FullClient {
	public TcpClient tcpClient;
	public User user = null!;

	public FullClient(TcpClient tcpClient){
		this.tcpClient = tcpClient;
	}
}
public partial class Server {

	public bool Login(User user){
		using Database db = new();
		User? userData = db.Users.Read(user.FileName);
		if (userData == null) return false;
		if (userData.Username == user.FileName && userData.Password == user.Password){
			return true;
		}
		return false;
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
                Console.WriteLine(m);
                Type type = (Type)m["type"];
                switch (type.ToString()){
                    case "User": 
                        model = JsonConvert.DeserializeObject<User>(JsonString);
						
                        break;
                    case "Message":
                        model = JsonConvert.DeserializeObject<Message>(JsonString);
                        
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
			//stream.Close();
			
		}
		return model;
	}

	
}