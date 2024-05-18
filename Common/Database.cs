using System.IO;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using Newtonsoft.Json;
using System.Linq;
public class Database : IDisposable {
    private static readonly string DatabaseDirectory = "Database";
    public static readonly string DatabasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, DatabaseDirectory);
    public ModelList<User> Users = new();
    public ModelList<Chat> Chats = new();

    //public Sequence UsersSequence = Sequence.Read("UsersSequence");
    
    public Database(){
        
    }

    public void Init(){
        Action<string> DictInit = (path) => {
            string name = Path.GetFileNameWithoutExtension(path);
            if (!Directory.Exists(path)){
                Directory.CreateDirectory(path);
                Console.WriteLine($"Created {name} folder in {path}");
            } else {
                Console.WriteLine($"Folder {name} already exists");
            }
        };
        DictInit(DatabasePath);
        DictInit(Users.FilePath);
        DictInit(Chats.FilePath);
        DictInit(Sequence.FilePath);
      
    }
    public void Dispose()
    {
        //SaveChanges();

    }
}


public class Sequence : Model {
    public static string FilePath = Path.Combine(Database.DatabasePath, "Sequences", typeof(Sequence).Name + 's');
    private uint _nextToGenerete;
    public uint Step;

    public Sequence(uint Start, uint Step, string FileName) : base(typeof(Model)){
        Current = Start;
        this.Step = Step;
        this.FileName = FileName;
    }
    public uint Current {
        get { return _nextToGenerete; }
        set { _nextToGenerete = value; }
    }
    public uint Next {
        get {
            uint Result = _nextToGenerete;
            if ((ulong)Result + (ulong)Step > uint.MaxValue){
                throw new Exception ("Sequence maximum level reached");
            }   
            _nextToGenerete += Step;
            return Result;
        }
    }

    public static Sequence Read(string FileName){
        string ModelPath = Path.Combine(Database.DatabasePath, "Sequences",  FileName + ".json");
        if (!File.Exists(ModelPath)){
            Console.WriteLine($"File {FileName}.json doesn't exists in the Sequences folder");
            return new Sequence(0, 1, FileName);
        }
        
        string jsonData = File.ReadAllText(ModelPath);
        Sequence? Result = JsonConvert.DeserializeObject<Sequence>(jsonData);

        if (Result == null) return new Sequence(0, 1, FileName);
        return Result;
    }

}