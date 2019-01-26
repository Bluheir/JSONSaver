using JSONSaver.Types;
using System.Collections.Generic;
using System.Linq;
namespace JSONSaver
{
    public class ValuesGetter<T, TKey> where T : class, ISaveable<TKey>
    {
        public List<T> Values;
        public string _path { get; }
        public T DefaultValue { get; set; }
        public ValuesGetter(string path)
        {
            _path = path;
            if (!DataStorage.FileExists(_path))
            {
                Values = new List<T>();
                SaveData();
            }
            else
            {
                Values = DataStorage.LoadData<List<T>>(_path);
            }
        }
        public void SaveData()
        {
            DataStorage.SaveData(Values, _path);
        }
        public T GetValue(TKey key)
        {
            var b = from a in Values
                    where a.Key.Equals(key)
                    select a;
            return b.FirstOrDefault();
        }
        public T GetOrCreateValue(TKey key)
        {
            var a = GetValue(key);
            if (a == null)
                return CreateValue(key);
            return a;
        }
        public T CreateValue(TKey key)
        {
            var a = DefaultValue;
            a.Key = key;
            Values.Add(a);
            SaveData();
            return a;
        }
    }
}