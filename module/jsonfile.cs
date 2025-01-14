using Newtonsoft.Json;
using System.IO;

public class ConfigureFile
{
public string pathFolder { get; set; }

}

public class JsonHelper
{
public static void WriteJson(string filePath, object objectToWrite) 
{
    File.WriteAllText(filePath, JsonConvert.SerializeObject(objectToWrite));
}

public static T ReadJson<T>(string filePath) 
{
    return JsonConvert.DeserializeObject<T>(File.ReadAllText(filePath));
}
}
