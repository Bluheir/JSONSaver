using Newtonsoft.Json;
using System.IO;
namespace JSONSaver.Types
{
    /// <summary>
    /// The class which saves the JSON data
    /// </summary>
    public static class DataStorage
    {
        /// <summary>
        /// Returns if that file path exists or not
        /// </summary>
        /// <param name="path">The file path</param>
        /// <returns>Returns true or false if the file path exists respectively</returns>
        public static bool FileExists(string path)
        {
            ISaveable<object> a;
            return File.Exists(path);
        }
        /// <summary>
        /// Saves the data for the specified object at the specified path
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value being saved</param>
        /// <param name="path">The file path</param>
        public static void SaveData<T>(T value, string path, Formatting formatting = Formatting.Indented)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(value, formatting));
        }
        /// <summary>
        /// Loads the data from the specified path
        /// </summary>
        /// <typeparam name="T">The type for the data</typeparam>
        /// <param name="path">The file path</param>
        /// <returns>The data from that file deserialized to the T</returns>
        public static T LoadData<T>(string path) where T : class
        {
            if (!FileExists(path))
                return null;
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(path));
        }
    }
}