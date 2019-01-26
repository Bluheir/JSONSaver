using Newtonsoft.Json;
using System.IO;
namespace JSONSaver.Types
{
    public static class DataStorage
    {
        public static bool FileExists(string path)
        {
            return File.Exists(path);
        }
        public static void SaveData<T>(T value, string path)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(value, Formatting.Indented));
        }
        public static T LoadData<T>(string path) where T : class
        {
            if (!FileExists(path))
                return null;
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}