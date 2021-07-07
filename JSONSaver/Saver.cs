using JSONSaver.Data;
using JSONSaver.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSONSaver
{
    /// <summary>
    /// The main class for saving data to a filepath in json
    /// </summary>
    /// <typeparam name="T">The ISaveable to be saved</typeparam>
    /// <typeparam name="TKey">The Type of the key of T</typeparam>
    public class JSONDatabase<T, TKey> where T : class, ISaveable<TKey>
    {
        private ConcurrentDictionary<TKey, T> dictValues;
		private List<T> _values;

        /// <summary>
        /// The main file path
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The default value when new values are created.
        /// </summary>
        public Func<TKey, T> DefaultValue { get; set; }
        /// <summary>
        /// The serialization type
        /// </summary>
        public SerializationType Serialization { get; set; } = SerializationType.Json;
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path">Path to create JSON file</param>
        /// <param name="defaultValue">Value factory when no value is found</param>
        /// <param name="serialization">Either XML or JSON serialization</param>
        /// <param name="formatting">Formatting type</param>
        public JSONDatabase(string path, Func<TKey, T> defaultValue, SerializationType serialization = SerializationType.Json, Formatting formatting = Formatting.None)
        {
            Path = path;
            Serialization = serialization;
            DefaultValue = defaultValue;
            JsonFormatting = formatting;

            dictValues = new ConcurrentDictionary<TKey, T>();

            if (!DataStorage.FileExists(Path))
            {
                _values = new List<T>();
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
                DataStorage.SaveData(_values, Path, JsonFormatting);
            else if (Serialization == SerializationType.Xml)
                DataStorage.SaveDataXml(Path, _values);
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
                return dictValues[key];
            }
        }

        /// <summary>
        /// Attempts to get the value associated with the specified key from the internal dictionary
        /// </summary>
        /// <param name="key">The key of the value to get</param>
        /// <param name="value">The value to return. Will be default value of type if operation fails</param>
        /// <returns></returns>
        public bool TryGetValue(TKey key, out T value)
        {
            var v = dictValues.TryGetValue(key, out value);

            return v;
        }
        /// <summary>
        /// Gets or creates a value with that specified key and returns it.
        /// </summary>
        /// <param name="key">The value with that key</param>
        /// <returns>Returns the first value with that key. If it doesn't exist, creates a new value with that key and the value of the default value and returns it.</returns>
        public T GetOrAdd(TKey key)
        {
            var v = TryGetValue(key, out var ret);

            if(!v)
            {
                var def = DefaultValue(key);
                def.Key = key;

                dictValues.TryAdd(key, def);
                _values.Add(def);

                return def;
			}

            return ret;
        }
        /// <summary>
        /// Saves all data to the file path asynchronously
        /// </summary>
        public async Task SaveDataAsync()
        {
            if (Serialization == SerializationType.Json)
                await DataStorage.SaveDataAsync(_values, Path, JsonFormatting);
            else if (Serialization == SerializationType.Xml)
                await DataStorage.SaveDataAsyncXml(Path, _values);
        }
        /// <summary>
        /// Reloads the values from the data file
        /// </summary>
        /// <returns>Returns an empty Task</returns>
        public async Task ReloadAsync()
        {
            if (Serialization == SerializationType.Json)
                _values = await DataStorage.LoadDataAsync<List<T>>(Path);
            else if (Serialization == SerializationType.Xml)
                _values = await DataStorage.LoadDataAsyncXML<List<T>>(Path);

            InitValuesDict();
        }
        /// <summary>
        /// Reloads the values from the data file
        /// </summary>
        public void Reload()
        {
            if (Serialization == SerializationType.Json)
                _values = DataStorage.LoadData<List<T>>(Path);
            else if (Serialization == SerializationType.Xml)
                _values = DataStorage.GetDataXml<List<T>>(Path);

            InitValuesDict();
        }


        private void InitValuesDict()
        {
            dictValues.Clear();
            foreach(var value in _values)
            {
                if (!dictValues.TryAdd(value.Key, value)) 
                {
                    throw new InvalidOperationException("Duplicate key exists in database");
				}
			}
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