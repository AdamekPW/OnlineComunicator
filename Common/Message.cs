using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



public class Message : Model, INotifyPropertyChanged
{
    private DateTime _date;
    private string _data = string.Empty;
    private string _username = string.Empty;
    private bool _isMyMessage = true;
    public DateTime Date { 
        get { return _date; } 
        set
        {
            _date = value;
            OnPropertyChanged(nameof(Date));
        } 
    }

    public string Data
    {
        get { return _data; }
        set 
        {
            _data = value;
            OnPropertyChanged(nameof(Data));
        }
    }

    public string Username
    {
        get { return _username; }
        set
        {
            _username = value;
            OnPropertyChanged(nameof(Username));
        }
    }

    public bool IsMyMessage
    {
        get { return _isMyMessage; }
        set
        {
            _isMyMessage = value;
            OnPropertyChanged(nameof(IsMyMessage));
        }
    }

    public Message() : base(typeof(Message))
    {
        this.Date = DateTime.Now;
    }

    public Message(string Data) : base(typeof(Message)){
        this.Data = Data;
        this.Date = DateTime.Now;
    }
    public Message(string Data, DateTime Date, string Username, bool IsMyMessage = false) : base(typeof(Message))
    {
        this.Data = Data;
        this.Date = Date;
        this.Username = Username;
        this.IsMyMessage = IsMyMessage;
    }

    public event PropertyChangedEventHandler? PropertyChanged;
    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}

