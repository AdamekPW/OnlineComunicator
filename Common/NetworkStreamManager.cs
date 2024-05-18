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