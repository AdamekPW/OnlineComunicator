
using System.Dynamic;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using System.Linq;

public class Model {
    
    public readonly Type type;
    public string FileName = "";

    
    public Model(){
        type = typeof(Model);
    }
    public Model(Type type){
        
        this.type = type;
    }

    public Model(string FileName, Type type){
        this.FileName = FileName;
        this.type = type;
    }

    public void Save(){
        string ModelPath = Path.Combine(Database.DatabasePath, GetType().ToString() + 's', FileName + ".json");
        string jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);
        File.WriteAllText(ModelPath, jsonData);
    }

    
    

}

public class ModelList<T>: List<T> where T : Model {
    public readonly string FilePath;

    public ModelList(){
        FilePath = Path.Combine(Database.DatabasePath, typeof(T).Name + 's');
    }

    public new void Add(T model){
        model.Save();
    }
    public T? Read(string FileName){

        string ModelPath = Path.Combine(FilePath, FileName + ".json");
        if (!File.Exists(ModelPath)){
            Console.WriteLine($"File {FileName}.json doesn't exists in the {typeof(T).Name}s folder");
            return null;
        }
        
        string jsonData = File.ReadAllText(ModelPath);
        T? Result = JsonConvert.DeserializeObject<T>(jsonData);

        return Result;
    }

    public void SaveAll(){
        Console.WriteLine($"Saving {typeof(T).Name}s...");
        foreach (T element in this){
            element.Save();
        }
        Console.WriteLine("Done");
    }

}

