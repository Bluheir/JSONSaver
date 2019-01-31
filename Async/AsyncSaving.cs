using JSONSaver.Data;
using JSONSaver.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JSONSaver.Async
{/// <summary>
/// The main class for saving data to a filepath in json asynchronously
/// </summary>
/// <typeparam name="T">The ISaveable to be saved</typeparam>
/// <typeparam name="TKey">The Type of the key of T</typeparam>
    public class AsyncValuesGetter<T, TKey> where T : class, ISaveable<TKey>
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
        /// The constructor for this class (calls a private async void)
        /// </summary>
        /// <param name="filepath">The file path for saving</param>
        public AsyncValuesGetter(string filepath)
        {
            Constructor(filepath);
        }
        private async void Constructor(string filepath)
        {
            Path = filepath;
            if (!DataStorage.FileExists(Path))
            {
                Values = new List<T>();
                await SaveDataAsync();
            }
            else
            {
                Values = await DataStorage.LoadDataAsync<List<T>>(Path);
            }
        }
        /// <summary>
        /// Saves all data to the file path asynchronously
        /// </summary>
        public async Task SaveDataAsync()
        {
            await DataStorage.SaveDataAsync(Values, Path);
        }
        /// <summary>
        /// Fetches a value based on the predicate
        /// </summary>
        /// <param name="predicate">The predicate</param>
        /// <returns>Returns a value with that predicate</returns>
        public IEnumerable<T> Where(Func<T, bool> predicate)
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
        public async Task<T> GetOrCreateValue(TKey key)
        {
            var a = GetValue(key);
            if (a == null)
            {
                var b = DefaultValue;
                b.Key = key;
                Values.Add(b);
                await SaveDataAsync();
                return b;
            }
            return a;
        }

    }
}