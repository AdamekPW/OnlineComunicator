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

    public string Password {
        get { return _password; }
        set 
        {
            _password = value;
            OnPropertyChanged(nameof(Password));
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
        this.Password = Password;
        this.FileName = Username;
    }

    public static User? Read(string FileName){
        
        string ModelPath = Path.Combine(Database.DatabasePath, typeof(User).Name + 's', FileName + ".json");
        if (!File.Exists(ModelPath)){
            Console.WriteLine($"File {FileName}.json doesn't exists in the {typeof(User).Name}s folder");
            return null;
        }
        
        string jsonData = File.ReadAllText(ModelPath);
        User? Result = JsonConvert.DeserializeObject<User>(jsonData);

        return Result;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

}