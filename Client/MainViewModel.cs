using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Net.Sockets;

namespace Communicator
{
	public class MainViewModel : INotifyPropertyChanged
	{
		
		//public CustomClient customClient = new();
		public FullClient fullClient;

		public User user = new("Adam", "Adasek");
		public MainViewModel()
		{
			fullClient = new FullClient(new TcpClient("127.0.0.1", 48025));
			fullClient.Send(user);
			Messages = new ObservableCollection<Message>() 
			{ new Message("Hello from C#", DateTime.Now, "Adam", false),
			  new Message("Yes yes yes", DateTime.Now, "Wiktoria", true)};
			

			
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

		
		public void AddMessage(Message message)
		{
			Messages.Add(message);
			
		}





		public event PropertyChangedEventHandler? PropertyChanged;

		protected virtual void OnPropertyChanged(string propertyName)
		{
			PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
		}
	}

}
