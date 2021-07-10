using JSONSaver.Data;
using JSONSaver.Types;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace JSONSaver
{
    /// <summary>
    /// The main class for saving data to a filepath in json
    /// </summary>
    /// <typeparam name="T">The ISaveable to be saved</typeparam>
    /// <typeparam name="TKey">The Type of the key of T</typeparam>
    public class JSONDatabase<T, TKey> : IReadOnlyDictionary<TKey, T>, IReadOnlyList<T> where T : class, ISaveable<TKey>
    {
        private ConcurrentDictionary<TKey, T> dictValues;
		private List<T> _values;

        /// <summary>
        /// The main file path
        /// </summary>
        public string Path { get; private set; }
        /// <summary>
        /// The serialization type
        /// </summary>
        public SerializationType Serialization { get; set; } = SerializationType.Json;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="path">Path to create JSON file</param>
        /// <param name="serialization">Either XML or JSON serialization</param>
        /// <param name="formatting">Formatting type</param>
        public JSONDatabase(string path, SerializationType serialization = SerializationType.Json, Formatting formatting = Formatting.None)
        {
            Path = path;
            Serialization = serialization;
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
        /// Keys enumerable
        /// </summary>
		public IEnumerable<TKey> Keys => (dictValues).Keys;
        /// <summary>
        /// Values enumerable
        /// </summary>
		public IEnumerable<T> Values => _values;
        /// <summary>
        /// Amount of items in JSON database
        /// </summary>
		public int Count => _values.Count;

        /// <summary>
        /// Returns an item of an index in JSON database
        /// </summary>
        /// <param name="index">Index to find</param>
        /// <returns>Item at specified index</returns>
		public T this[int index] => _values[index];

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
        /// Returns if the key exists in the internal dictionary
        /// </summary>
        /// <param name="key">The key</param>
        /// <returns>If the key exists in the dictionary</returns>
        public bool ContainsKey(TKey key)
        {
            return dictValues.ContainsKey(key);
		}

        /// <summary>
        /// Attempts to get the value associated with the specified key from the internal dictionary
        /// </summary>
        /// <param name="key">The key of the value to get</param>
        /// <param name="value">The value to return. Will be default value of type if operation fails</param>
        /// <returns>If the value was successfully returned</returns>
        public bool TryGetValue(TKey key, out T value)
        {
            var v = dictValues.TryGetValue(key, out value);

            return v;
        }
		/// <summary>
		/// Gets or creates a value with that specified key and returns it.
		/// </summary>
		/// <param name="key">The value with that key</param>
		/// <param name="valueFactory">Value factory to add</param>
		/// <returns>Returns the first value with that key. If it doesn't exist, creates a new value with that key and the value of the default value and returns it.</returns>
		public T GetOrAdd(TKey key, Func<TKey, T> valueFactory)
        {
            var v = TryGetValue(key, out var ret);

            if(!v)
            {
                var def = valueFactory(key);
                def.Key = key;

                dictValues.TryAdd(key, def);
                _values.Add(def);

                return def;
			}

            return ret;
        }
        /// <summary>
        /// Trys to remove a value from data
        /// </summary>
        /// <param name="key">Value with key to remove</param>
        /// <param name="value">Value that was removed</param>
        /// <returns></returns>
        public bool TryRemove(TKey key, out T value)
        {
            if (!dictValues.TryRemove(key, out value))
            {
                value = default;
                return false;
            }
            
            _values.Remove(value);

            return true;
		}
        /// <summary>
        /// Gets or creates a value with that specified key and returns it.
        /// </summary>
        /// <param name="key">The value with that key</param>
        /// <param name="value">Value to add if key doesn't exist</param>
        /// <returns></returns>
        public T GetOrAdd(TKey key, T value)
        {
            var v = TryGetValue(key, out var ret);

            if (!v)
            {
                var def = value;
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
        /// <summary>
        /// Returns enumerator
        /// </summary>
        /// <returns>Enumerator for all items in collection</returns>
		public IEnumerator<T> GetEnumerator()
		{
            return _values.GetEnumerator();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
            return GetEnumerator();
		}

		IEnumerator<KeyValuePair<TKey, T>> IEnumerable<KeyValuePair<TKey, T>>.GetEnumerator()
		{
            return dictValues.GetEnumerator();
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