using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;

namespace Communicator
{
	public class MainViewModel : INotifyPropertyChanged
	{

		//public CustomClient customClient = new();
		public FullClient fullClient;

		public User Me = new("Adam", "Adasek");
		public MainViewModel()
		{
			
			

			
		}
		public void Init()
		{
			fullClient = new("127.0.0.1", 48025);
			fullClient.Send(Me);
			Messages = new ObservableCollection<Message>();
			Task.Run(() =>
			{
				while (true)
				{
					while (!fullClient.IsDataAvailable) { };
					Model model = fullClient.Data;
					if (model.type == typeof(Message))
					{
						Message message = (Message)model;

						if (message.Username == Me.Username) message.IsMyMessage = true;
						else message.IsMyMessage = false;

						AddMessage(message);

					}
					
				}
			});
		}


		private ObservableCollection<Message> _messages;
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
