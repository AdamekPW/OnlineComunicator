using System.Net.Sockets;
using System.Text;
using Newtonsoft.Json;
public class NetworkStreamManager {
    private int bufferSize = 1024;
    public NetworkStream stream = null!;

    public NetworkStreamManager(){}
    public NetworkStreamManager(NetworkStream stream){
        this.stream = stream;
    }
    ~NetworkStreamManager(){
        stream.Close();
    }
    public void Send(Model model){                   
        try {
        
            string jsonData = JsonConvert.SerializeObject(model);
            byte[] jsonDataBytes = Encoding.UTF8.GetBytes(jsonData);

            int bytesSent = 0;
            while (bytesSent < jsonDataBytes.Length)
            {
                int remainingBytes = jsonDataBytes.Length - bytesSent;
                int bytesToSend = Math.Min(bufferSize, remainingBytes);
                stream.Write(jsonDataBytes, bytesSent, bytesToSend);
                bytesSent += bytesToSend;

            }
            
        } catch (Exception e){
            Console.WriteLine("Error: " + e.Message);
        } 
    }

    public Model? Read(){
        Model? model = null;
		try
		{	
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
        
        return model;
    }
    public void SendASCII(string data){
        byte[] msg = Encoding.ASCII.GetBytes(data);
        stream.Write(msg);
    }
    public string ReadASCII(){
        byte[] data = new byte[256];
        int bytes = stream.Read(data, 0, data.Length);
        return Encoding.ASCII.GetString(data, 0, bytes);
    }
    
}