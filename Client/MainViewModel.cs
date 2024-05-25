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
		public Client client;
		private Task ServerTask;
		public User Me = new("Adam", "Adasek");
		public MainViewModel()
		{
			
			

			
		}
		public void Init()
		{
			client = new("127.0.0.1", 48025);
			client.Send(Me);
			Messages = new ObservableCollection<Message>();
			ServerTask = Task.Run(() =>
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
