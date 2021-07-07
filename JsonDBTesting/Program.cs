using System;
using JSONSaver;
using JSONSaver.Types;

namespace JsonDBTesting
{
	class Program
	{
		static void Main(string[] args)
		{
			var db = new JSONDatabase<TestClass, string>("C:/Users/slimc/Desktop/data.json", a => new TestClass(), formatting: Newtonsoft.Json.Formatting.Indented);

			var value = db.GetOrAdd("CoolGuy1234");

			db.SaveData();

			Console.WriteLine($"Edit the json file. Will print password after you press enter. Current password is {value.Password}");
			Console.ReadLine();

			db.Reload();

			var newvalue = db.GetOrAdd("CoolGuy1234"); // The instance is new

			Console.WriteLine($"New password is: {newvalue.Password}");

			Console.WriteLine("Do you want to change the password (y/n)?");

			var k = Console.ReadLine().ToLower();
			
			if(k == "y")
			{
				Console.WriteLine("What do you want to replace the password with?");

				newvalue.Password = Console.ReadLine();
				db.SaveData();

				Console.WriteLine("Changed password");
				
			}

			Console.ReadKey();
		}
	}

	class TestClass : ISaveable<String>
	{
		public string Username { get; set; }
		public string Password { get; set; }
		string ISaveable<string>.Key { get => Username; set => Username = value; }
	}
}
