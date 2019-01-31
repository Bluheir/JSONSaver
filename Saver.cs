using JSONSaver.Data;
using JSONSaver.Types;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// The constructor for this class
        /// </summary>
        /// <param name="path">The json file path</param>
        public ValuesGetter(string path)
        {
            Path = path;
            if (!DataStorage.FileExists(Path))
            {
                Values = new List<T>();
                SaveData();
            }
            else
            {
                Values = DataStorage.LoadData<List<T>>(Path);
            }
        }
        /// <summary>
        /// Saves all data to the file path
        /// </summary>
        public void SaveData()
        {
            DataStorage.SaveData(Values, Path);
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
                SaveData();
                return b;
            }
            return a;
        }
        
    }
}