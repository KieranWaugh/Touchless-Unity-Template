using System.Collections.Generic;
using System.IO;
using UnityEngine;


public abstract class Logger : MonoBehaviour
{
    private List<object> logs;
    protected string CreateJSON<T>(T log){
        
        return JsonUtility.ToJson(log, true);
    }

    protected string CreateJSONFromList(){
        
        return JsonUtility.ToJson(logs, true);
    }
    protected void export(string path, string fileName, string json){
        Directory.CreateDirectory(path);
        File.WriteAllText(path + fileName, json);
    }

    protected void Log<T>(T log){
        logs.Add(log);
    }

    void Update(){
        if(Settings.enable_logging){
            updateLog();
        }
    }

    public abstract void updateLog(); 
}
