using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

public class FullClient : NetworkStreamManager {
	public TcpClient tcpClient;
	public User user = null!;

	//for sending data
	private bool _isSendDataAvailable = false;
	private ReaderWriterLockSlim _isSendDataAvailableLock = new();
	private bool IsSendDataAvailable {
		get {
			_isSendDataAvailableLock.EnterReadLock();
			bool res = _isSendDataAvailable;
			_isSendDataAvailableLock.ExitReadLock();
			return res;
		}
		set {
			_isSendDataAvailableLock.EnterWriteLock();
			_isSendDataAvailable = value;
			_isSendDataAvailableLock.ExitWriteLock();
		}
	}
	private Queue<Model> _sendDataQueue = new();
	private SemaphoreSlim _sendDataQueueLock = new(1);
	public new void Send(Model model){
		_sendDataQueueLock.Wait();
		_sendDataQueue.Enqueue(model);
		IsSendDataAvailable = true;
		_sendDataQueueLock.Release();
	}	
	


	//for receiving data
	private bool _isDataAvailable = false;
	private ReaderWriterLockSlim _isDataAvailableLock = new();
	public bool IsDataAvailable {
		get {
			_isDataAvailableLock.EnterReadLock();
			bool res = _isDataAvailable;
			_isDataAvailableLock.ExitReadLock();
			return res;
		}
		set {
			_isDataAvailableLock.EnterWriteLock();
			_isDataAvailable = value;
			_isDataAvailableLock.ExitWriteLock();
		}
	}

	private Queue<Model> _receiveDataQueue = new();
	private SemaphoreSlim _receiveDataQueueLock = new(1);
	public Model Data {
		get {
			_receiveDataQueueLock.Wait();
			Model model = _receiveDataQueue.Dequeue();
			if (_receiveDataQueue.Count == 0) IsDataAvailable = false;
			_receiveDataQueueLock.Release();
			return model;
		}
		set {
			_receiveDataQueueLock.Wait();
			_receiveDataQueue.Enqueue(value);
			IsDataAvailable = true;
			_receiveDataQueueLock.Release();
		}
	}


	private Task _sendTask = null!; 
	private Task _receiveTask = null!;
	private bool _isSendTaskShouldEnd, _isReceiveTaskShouldEnd;
	public void Run(){
		_sendTask = Task.Run(() => {
			while (!_isSendTaskShouldEnd){
				if (IsSendDataAvailable){
					_sendDataQueueLock.Wait();
					Model model = _sendDataQueue.Dequeue();
					if (_sendDataQueue.Count == 0) IsSendDataAvailable = false;
					_sendDataQueueLock.Release();
					base.Send(model);
				}
			}
			
		});
		_receiveTask = Task.Run(() => {
			while (!_isReceiveTaskShouldEnd){
				if (stream.CanRead && stream.DataAvailable){
					
					Model? model = Read();
					if (model == null) continue;
					Data = model;
				} 
			}
		});

		Task.Run(async () => {
			while (true){
				Console.WriteLine("chodzi");
				Send(new Message("Hello"));
				await Task.Delay(10000);
				
			}
		});
	}
	public void Stop(){
		_isSendTaskShouldEnd = true;
		_isReceiveTaskShouldEnd = true;
	}
	

	public FullClient(TcpClient tcpClient){
		this.tcpClient = tcpClient;
		this.stream = tcpClient.GetStream();
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

	
    public static Model? HandleClient(TcpClient client)
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