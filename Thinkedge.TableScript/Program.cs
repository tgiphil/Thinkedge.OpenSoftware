using System;
using System.IO;
using Thinkedge.Simple.Script;
using Thinkedge.Simple.Script.Processing;

namespace Thinkedge.TableScript
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			Console.WriteLine("TableScript. (C)opyright 2017. GPLv3 License.");
			Console.WriteLine("Usage: [-file filename] [arg1] [arg2] ...");
			Console.WriteLine();

			var script = new ScriptInterpreter();

			script.AddMethodSources(new FileMethods());
			script.AddMethodSources(new TableMethods());
			script.AddMethodSources(new EMailMethods());

			if (args.Length >= 1 && args[0] == "-file")
			{
				if (args.Length < 2)
				{
					Console.WriteLine("Error: Missing filename");
					return 1;
				}

				var scriptData = File.ReadAllLines(args[1]);

				for (int i = 2; i < args.Length; i++)
				{
					script.SetVariableArgument(i - 1, args[i]);
				}

				foreach (var line in scriptData)
				{
					Console.WriteLine("> " + line);

					script.Execute(line);

					if (script.HasError)
					{
						Console.WriteLine("Error: " + script.ErrorMessage);
						return 1;
					}
				}

				return 0;
			}
			else
			{
				while (true)
				{
					for (int i = 0; i < args.Length; i++)
					{
						script.SetVariableArgument(i + 1, args[i]);
					}

					Console.Write(" >");

					var line = Console.ReadLine().TrimEnd();

					if (line == "quit" || line == "exit")
					{
						return 0;
					}

					script.Execute(line);

					if (script.HasError)
					{
						Console.WriteLine("Error: " + script.ErrorMessage);
						return 1;
					}
				}
			}

			return 1;
		}
	}
}