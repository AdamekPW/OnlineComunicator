using Newtonsoft.Json;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Text;

public class Client : NetworkStreamManager {
	public TcpClient tcpClient;
	public User user = null!;
	public Client(string ServerIP, int ServerPort){
		tcpClient = new TcpClient(ServerIP, ServerPort);
		this.stream = tcpClient.GetStream();
		Run();
	}
	public Client(TcpClient tcpClient){
		this.tcpClient = tcpClient;
		this.stream = tcpClient.GetStream();
		Run();
	}
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
			if (_receiveDataQueue.Count == 0) throw new Exception("No data available");
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


	private Task _runTask = null!; 
	private bool _isRunTaskShouldEnd;

	public void Run(){
		_runTask = Task.Run(async () => {
			while (!_isRunTaskShouldEnd){
				if (IsSendDataAvailable){
					_sendDataQueueLock.Wait();
					Model model = _sendDataQueue.Dequeue();
					if (_sendDataQueue.Count == 0) IsSendDataAvailable = false;
					_sendDataQueueLock.Release();
					base.Send(model);
				}
				if (stream.CanRead && stream.DataAvailable){
					Model? model = base.Read();
					if (model == null) continue;
					Data = model;
					
				}
				await Task.Delay(50);
			
			}
			
		});
		
	}
	public void Stop(){
		_isRunTaskShouldEnd = true;
	}
	

}