using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Communicator
{
	public class MainViewModel : INotifyPropertyChanged
	{
		

		

		public MainViewModel()
		{
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
