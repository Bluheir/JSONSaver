# JsonSaver
This is an easy way to store JSON array data to .json files.
### How to use
The default namespace for JSONSaver JSONSaver
The namespace for the interface ISaveable is JSONSaver.Types
The namespace for async saving is JSONSaver.Async
If you want, the namespace for saving data, converting it, etc is JSONSaver.Data
```
public class TestClass1234 : ISaveable<string>
{
  public string Key { get; set; }
  public int Age { get; set; }
}
///
static void Main()
{
  var a = new ValuesGetter<TestClass1234, string>(@"C:\Users\User\Desktop\test.json"); //The filepath can be anywhere
  a.DefaultValue = new TestClass1234
  {
    Age = 20
  };
  Console.WriteLine(a.GetOrCreateValue("Sam").Age);
  var b = a.GetOrCreateValue("Tom");
  b.Age = 25;
  Console.WriteLine(a.GetValue("Tom").Age);
  a.SaveData();
  // Remember to do ValuesGetter<T, TKey>#SaveData() or the data won't be saved
}
//Output
20
25


//This is what test.json looks like
[
	{
		"Key" : "Sam",
		"Age" : 20
	},
	{
		"Key" : "Tom",
		"Age" : 25
	}
]
```
