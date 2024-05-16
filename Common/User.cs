using System.ComponentModel;
using System.Collections.ObjectModel;
using Newtonsoft.Json;


public class User: Model, INotifyPropertyChanged {
    private string _username = null!;
    private string _password = null!;
    private ObservableCollection<string> _chats = new();
    public string Username {
        get { return _username; }
        set 
        { 
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public ObservableCollection<string> Chats {
        get { return _chats; }
        set 
        {
            _chats = Chats;
            OnPropertyChanged(nameof(Chats));
        }
    }


    public User(string Username, string Password): base(typeof(User)){
        this.Username = Username;
        this._password = Password;
        this.FileName = Username;
    }



    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}