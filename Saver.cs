using JSONSaver.Types;
using System.Collections.Generic;
using System.Linq;
namespace JSONSaver
{
    /// <summary>
    /// The class for mainly loading and saving json data, and getting the data. 
    /// </summary>
    /// <typeparam name="T">The ISaveable<T> to be used</typeparam>
    /// <typeparam name="TKey">The key (must be the same type as T's Key)</typeparam>
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