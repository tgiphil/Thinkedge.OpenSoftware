using System;
using System.IO;
using Thinkedge.Simple.ScriptEngine;
using Thinkedge.TableScript.Method;

namespace Thinkedge.TableScript
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			Console.WriteLine("TableScript V1.1");
			Console.WriteLine("Copyright (C) 2017. Phil Garcia. All rights reserved.");
			Console.WriteLine("Licenced under GPLv3 License.");
			Console.WriteLine("Usage: [-file filename] [arg1] [arg2] ...");
			Console.WriteLine();

			var script = new ScriptInterpreter();

			script.AddMethodSources(new FileMethods());
			script.AddMethodSources(new TableMethods());
			script.AddMethodSources(new EMailMethods());
			script.AddMethodSources(new SharePointMethods());

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

					var result = script.Execute(line);

					if (result.HasError)
					{
						Console.WriteLine("Error: ");
						Console.WriteLine(result.ErrorMessage);
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

					Console.Write("> ");

					var line = Console.ReadLine().TrimEnd();

					if (line == "quit" || line == "exit")
					{
						return 0;
					}

					var result = script.Execute(line);

					if (result.HasError)
					{
						Console.WriteLine("Error: ");
						Console.WriteLine(result.ErrorMessage);
						return 1;
					}
					else
					{
						Console.WriteLine("Result: " + result.Result.ToString());
					}
				}
			}
		}
	}
}