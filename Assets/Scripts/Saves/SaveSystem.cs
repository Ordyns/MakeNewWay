using System.IO;
using UnityEngine;

public class SaveSystem<T> where T: ISaveable, new()
{
    private readonly string _fullPath;
    private readonly string _targetDirectory;
    private readonly string _fileName = new T().FileName + ".json";

    public SaveSystem(){
        string editorPath = Path.Combine(Application.dataPath, "Saves/");
        _targetDirectory = Application.isEditor ? editorPath : Application.persistentDataPath;
        _fullPath = Path.Combine(_targetDirectory, _fileName);
    }

    public T LoadData(){
        if (Directory.Exists(_targetDirectory) == false || File.Exists(_fullPath) == false)
            return new T();

        return JsonUtility.FromJson<T>(File.ReadAllText(_fullPath)) ?? new T();
    }

    public bool TryLoadData(out T data){
        data = LoadData();
        return data != null;
    }

    public void SaveData(T data){
        if(Directory.Exists(_targetDirectory) == false)
            Directory.CreateDirectory(_targetDirectory);

        File.WriteAllText(_fullPath, JsonUtility.ToJson(data));
    }
}