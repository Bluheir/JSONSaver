using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;

namespace JSONSaver.Data
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
            return File.Exists(path);
        }
        /// <summary>
        /// Saves a value to the file path in json
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value being saved</param>
        /// <param name="path">The file path to save the data</param>
        /// <param name="formatting">The formatting</param>
        public static void SaveData<T>(T value, string path, Formatting formatting = Formatting.Indented)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(value, formatting));
        }
        /// <summary>
        /// Saves data asynchronously to json and the file path
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="value">The value to be saved</param>
        /// <param name="path">The file location to save the value to</param>
        /// <param name="formatting">The formatting</param>
        /// <returns></returns>
        public static async Task SaveDataAsync<T>(T value, string path, Formatting formatting = Formatting.Indented)
        {
            await WriteAsync(JsonConvert.SerializeObject(value, formatting), path);
        }
        /// <summary>
        /// Writes the data to the specified location asynchronously
        /// </summary>
        /// <param name="data">The string to be written</param>
        /// <param name="filepath">The file location</param>
        /// <returns></returns>
        public static async Task WriteAsync(string data, string filepath)
        {
            using (var sw = new StreamWriter(filepath))
            {
                await sw.WriteAsync(data);
            }
        }
        /// <summary>
        /// Loads the data from the specified path
        /// </summary>
        /// <typeparam name="T">The type for the data</typeparam>
        /// <param name="filepath">The file path</param>
        /// <returns>The data from that file deserialized to the T</returns>
        public static T LoadData<T>(string filepath) where T : class
        {
            if (!FileExists(filepath))
                return null;
            return JsonConvert.DeserializeObject<T>(File.ReadAllText(filepath));
        }
        
        /// <summary>
        /// Reads all text from the specified file path asynchronously
        /// </summary>
        /// <param name="filepath">The file location</param>
        /// <returns>Returns the text from that file</returns>
        public static async Task<string> ReadAllTextAsync(string filepath)
        {
            using (var reader = File.OpenText(filepath))
            {
                return await reader.ReadToEndAsync();
            }
        }
        /// <summary>
        /// Loads the data from the file from json to the specified type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="filepath">The filepath</param>
        /// <returns>Returns the text deserialized to the type of T</returns>
        public static async Task<T> LoadDataAsync<T>(string filepath) where T : class
        {
            if (!FileExists(filepath))
                return null;
            return JsonConvert.DeserializeObject<T>(await ReadAllTextAsync(filepath));
        }
    }
}