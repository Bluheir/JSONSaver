# JsonSaver
This is an easy way to store JSON array data.
### How to use
```
public class TestClass1234 : ISaveable<string>
{
  public string Key { get; set; }
  public int Age { get; set; }
}
///
static void Main()
{
  var a = new ValuesGetter<TestClass1234, string>(@"C:\Users\User\Desktop\test.json");
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


//This is what test.json should look like
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
