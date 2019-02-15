# JsonSaver
This is an easy way to store JSON array data to .json files.
### How to use
The default namespace for JSONSaver JSONSaver

The namespace for the interface ISaveable is JSONSaver.Types
If you want, the namespace for saving data, converting it, etc is JSONSaver.Data

I made the default value part of the constructor now
```
public class TestClass1234 : ISaveable<string>
{
  public string Key { get; set; }
  public int Age { get; set; }
}
///
static void Main()
{
  var a = new ValuesGetter<TestClass1234, string>(@"C:\Users\User\Desktop\test.json",new TestClass1234
  {
    Age = 20
  }); //The filepath can be anywhere
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
