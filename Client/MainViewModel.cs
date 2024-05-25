using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Net.Sockets;
using Accessibility;

namespace Communicator
{
	public class MainViewModel : INotifyPropertyChanged
	{

		//public CustomClient customClient = new();
		public Client client = new("127.0.0.1", 48025);
		public User Me = new("Adam", "Adasek");
		
		
		public MainViewModel()
		{
			Messages = new ObservableCollection<Message>();
			Chats = new ObservableCollection<Message>() {
				new Message("Chat1"),
				new Message("Chat2"),
				new Message("Chat3"),
				new Message("Chat4")
			};
			
		}
		public void Init()
		{			
			client.Connect();
			if (!client.IsConnected) return;

			client.Send(Me);
			Task.Run(() =>
			{

				while (true)
				{

					while (!client.IsDataAvailable) { };
					Model model = client.Data;
					if (model.type == typeof(Message))
					{
						Message message = (Message)model;

						if (message.Username == Me.Username) message.IsMyMessage = true;
						else message.IsMyMessage = false;

						Application.Current.Dispatcher.Invoke(() => AddMessage(message));

					}

				}
			});
			
		}

		private ObservableCollection<Message> _chats = new();
		public ObservableCollection<Message> Chats
		{
			get { return _chats; }
			set
			{
				_chats = value;
				OnPropertyChanged(nameof(Chats));
			}
		}

		private ObservableCollection<Message> _messages = new();
		public ObservableCollection<Message> Messages
		{
			get { return _messages; }
			set { 
				_messages = value; 
				OnPropertyChanged(nameof(Messages));
			}
		}

		private SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1);
		public void AddMessage(Message message)
		{
			semaphoreSlim.Wait();
			Messages.Add(message);
			semaphoreSlim.Release();
		}


		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
