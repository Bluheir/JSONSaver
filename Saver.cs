using JSONSaver.Data;
using JSONSaver.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSONSaver
{/// <summary>
/// The main class for saving data to a filepath in json
/// </summary>
/// <typeparam name="T">The ISaveable to be saved</typeparam>
/// <typeparam name="TKey">The Type of the key of T</typeparam>
    public class ValuesGetter<T, TKey> where T : class, ISaveable<TKey>
    {
        /// <summary>
        /// The values
        /// </summary>
        public List<T> Values;
        /// <summary>
        /// The main file path
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The default value when new values are created.
        /// </summary>
        public T DefaultValue { get; set; }
        /// <summary>
        /// The serialization type
        /// </summary>
        public SerializationType Serialization { get; set; } = SerializationType.Json;
        /// <summary>
        /// The constructor for this class
        /// </summary>
        /// <param name="path">The json file path</param>
        /// <param name="async">The constructor is asynchronous</param>
        /// <param name="serialization">The serialization type. Can either be Xml or Json</param>
        /// <param name="formatting">The Json formatting</param>
        /// <param name="defaultValue">The default value</param>
        public ValuesGetter(string path, T defaultValue, SerializationType serialization = SerializationType.Json, bool async = false, Formatting formatting = Formatting.Indented)
        {
            Path = path;
            Serialization = serialization;
            DefaultValue = defaultValue;
            if (async)
            {
                _constructor();
                return;
            }
            if (!DataStorage.FileExists(Path))
            {
                Values = new List<T>();
                SaveData();
            }
            else
            {
                Reload();
            }
        }
        /// <summary>
        /// The Json formatting to be used
        /// </summary>
        public Formatting JsonFormatting { get; set; } = Formatting.Indented;
        /// <summary>
        /// Saves all data to the file path
        /// </summary>
        public void SaveData()
        {
            if (Serialization == SerializationType.Json)
                DataStorage.SaveData(Values, Path, JsonFormatting);
            else if(Serialization == SerializationType.Xml)
                DataStorage.SaveDataXml(Path, Values);
        }
        /// <summary>
        /// An indexer for the values
        /// </summary>
        /// <param name="key">The key to get</param>
        /// <returns>Returns a T of the specified key</returns>
        public T this[TKey key]
        {
            get
            {
                return GetValue(key);
            }
            set
            {
                try
                {
                    Values[Values.IndexOf(GetValue(key))] = value;
                }
                catch(Exception e)
                {
                    throw new KeyNotFoundException("That key does not exist", e);
                }
            }
        }
        /// <summary>
        /// Fetches a value based on the predicate
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>Returns a value with that predicate</returns>
        public IEnumerable<T> Where(Func<T,bool> predicate)
        {
            return Values.Where(predicate);
        }
        /// <summary>
        /// Gets a value the first value with that key
        /// </summary>
        /// <param name="key">The value with that key</param>
        /// <returns>Returns the first value in Values with that key. Otherwise returns null.</returns>
        public T GetValue(TKey key)
        {
            var b = from a in Values
                    where a.Key.Equals(key)
                    select a;
            return b.FirstOrDefault();
        }
        /// <summary>
        /// Gets or creates a value with that specified key and returns it.
        /// </summary>
        /// <param name="key">The value with that key</param>
        /// <returns>Returns the first value with that key. If it doesn't exist, creates a new value with that key and the value of the default value and returns it.</returns>
        public T GetOrCreateValue(TKey key)
        {
            var a = GetValue(key);
            if (a == null)
            {
                var b = DefaultValue;
                b.Key = key;
                Values.Add(b);
                return b;
            }
            return a;
        }
        private async void _constructor()
        {
            if (!DataStorage.FileExists(Path))
            {
                Values = new List<T>();
                await SaveDataAsync();
            }
            else
            {
                await ReloadAsync();
            }
        }
        /// <summary>
        /// Saves all data to the file path asynchronously
        /// </summary>
        public async Task SaveDataAsync()
        {
            if (Serialization == SerializationType.Json)
                await DataStorage.SaveDataAsync(Values, Path, JsonFormatting);
            else if (Serialization == SerializationType.Xml)
                await DataStorage.SaveDataAsyncXml(Path,Values);
        }
        /// <summary>
        /// Reloads the values from the data file
        /// </summary>
        /// <returns>Returns an empty Task</returns>
        public async Task ReloadAsync()
        {
            if(Serialization == SerializationType.Json)
                Values = await DataStorage.LoadDataAsync<List<T>>(Path);
            else if(Serialization == SerializationType.Xml)
                Values = await DataStorage.LoadDataAsyncXML<List<T>>(Path);
        }
        /// <summary>
        /// Reloads the values from the data file
        /// </summary>
        public void Reload()
        {
            if (Serialization == SerializationType.Json)
                Values = DataStorage.LoadData<List<T>>(Path);
            else if (Serialization == SerializationType.Xml)
                Values = DataStorage.GetDataXml<List<T>>(Path);
        }

    }
    /// <summary>
    /// An enum of the value Xml or value of Json
    /// </summary>
   
    public enum SerializationType
    {
        /// <summary>
        /// Xml format
        /// </summary>
        Xml,
        /// <summary>
        /// Json format
        /// </summary>
        Json
    }

}