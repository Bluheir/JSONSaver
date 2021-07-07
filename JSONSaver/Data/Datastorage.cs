using Newtonsoft.Json;
using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;

namespace JSONSaver.Data
{
    /// <summary>
    /// Provides useful methods to serialize an object to a filepath and deserialize from a filepath
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
        /// <param name="value">The value being saved</param>
        /// <param name="path">The file path to save the data</param>
        /// <param name="formatting">The formatting</param>
        public static void SaveData(object value, string path, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.Indented)
        {
            File.WriteAllText(path, JsonConvert.SerializeObject(value, formatting));
        }
        /// <summary>
        /// Saves data asynchronously to json and the file path
        /// </summary>
        /// <param name="value">The value to be saved</param>
        /// <param name="path">The file location to save the value to</param>
        /// <param name="formatting">The formatting</param>
        /// <returns></returns>
        public static async Task SaveDataAsync(object value, string path, Newtonsoft.Json.Formatting formatting = Newtonsoft.Json.Formatting.Indented)
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
        /// <summary>
        /// Converts the string from to the specified type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="objectData">The XML string</param>
        /// <returns>Returns the string from XML to the specified type</returns>
        public static T XmlDeserializeFromString<T>(string objectData)
        {
            return (T)XmlDeserializeFromString(objectData, typeof(T));
        }
        private static object XmlDeserializeFromString(string objectData, Type type)
        {
            var serializer = new XmlSerializer(type);
            object result;

            using (TextReader reader = new StringReader(objectData))
            {
                result = serializer.Deserialize(reader);
            }

            return result;
        }
        /// <summary>
        /// Loads the XML string from the specified file path and converts it to the specified type asynchronously
        /// </summary>
        /// <typeparam name="T">The Type to be converted to</typeparam>
        /// <param name="filepath">The file path</param>
        /// <returns>Returns the XML file text converted to the specified Type</returns>
        public static async Task<T> LoadDataAsyncXML<T>(string filepath) where T : class
        {
            if (!FileExists(filepath))
                return null;
            return XmlDeserializeFromString<T>(await ReadAllTextAsync(filepath));
            
        }
        /// <summary>
        /// Converts the object instance to a string based on XML format
        /// </summary>
        /// <param name="objectInstance">The object instance</param>
        /// <returns>Returns a string serialized from the object instance</returns>
        public static string XmlSerializeToString(object objectInstance)
        {
            var serializer = new XmlSerializer(objectInstance.GetType());
            var sb = new StringBuilder();

            using (TextWriter writer = new StringWriter(sb))
            {
                serializer.Serialize(writer, objectInstance);
            }

            return sb.ToString();
        }
        /// <summary>
        /// Serializes the value to XML and saves it to the file path asynchronously
        /// </summary>
        /// <param name="filePath">The file location</param>
        /// <param name="value">The object to be serialized</param>
        /// <returns>Returns Task</returns>
        public static async Task SaveDataAsyncXml(string filePath, object value)
        {
            await WriteAsync(XmlSerializeToString(value), filePath);
        }
        /// <summary>
        /// Serializes the value to XML and saves it to the file path
        /// </summary>
        /// <param name="filePath">The file location</param>
        /// <param name="value">The object to be serialized</param>
        public static void SaveDataXml(string filePath, object value)
        {
            File.WriteAllText(filePath, XmlSerializeToString(value));
        }
        /// <summary>
        /// Reads all text from the file path and deserializes it to the specified type
        /// </summary>
        /// <typeparam name="T">The type</typeparam>
        /// <param name="filePath">The file path</param>
        /// <returns>Returns the deserialized text from the file</returns>
        public static T GetDataXml<T>(string filePath) where T : class
        {
            if (!FileExists(filePath))
                return null;
            return XmlDeserializeFromString<T>(File.ReadAllText(filePath));
        }
    }
}