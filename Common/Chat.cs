using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;
using System.IO;
public class Chat : Model {
    public uint id;
    public string name = "MyChat";
    public List<string> Users;
    public LinkedList<Message> Messages = new LinkedList<Message>();
    private LinkedListNode<Message>? HistoryPointer = null;

    public Chat() : base(typeof(Chat)){
        Users = new List<string>();
    }

    public Chat(string User1, string User2) : base(typeof(Chat)){
        Users = new List<string>(){ User1, User2 };
        CreateFileName(Users);     
    }
    
    public Chat(List<string> Users) : base(typeof(Chat)){
        this.Users = Users;
        CreateFileName(Users);
    }

    public void AddMessage(Message message){
        Messages.AddLast(message);
    }

    public List<Message> GetHistory(int n){
        if (HistoryPointer == null){
            HistoryPointer = Messages.Last;
        }
        List<Message> messages = new List<Message>();
        while (HistoryPointer != null && n-- > 0){
            messages.Add(HistoryPointer.Value);
            HistoryPointer = HistoryPointer.Previous;
        }
        return messages;
    }

    private void CreateFileName(List<string> Users){
        Users.Sort();
        string input = "";
        string ChatFileName;
        foreach (string user in Users) input += user+"::";

        using (SHA256 sha256Hash = SHA256.Create())
        {
            byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i++)
                builder.Append(bytes[i].ToString("x2"));
            
            ChatFileName = builder.ToString();
        }
        this.FileName = ChatFileName;
    }
    

    
    public void Print(){
        foreach (var item in Messages){
            Console.WriteLine(item.Data);
        }
    }
}