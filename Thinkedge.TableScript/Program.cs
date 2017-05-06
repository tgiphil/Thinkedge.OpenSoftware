using System;
using System.IO;
using Thinkedge.Simple.Script;
using Thinkedge.Simple.Script.Process;

namespace Thinkedge.TableScript
{
	internal class Program
	{
		private static int Main(string[] args)
		{
			Console.WriteLine("TableScript V1.0");
			Console.WriteLine("Copyright (C) 2017. Phil Garcia. All rights reserved.");
			Console.WriteLine("Licenced under GPLv3 Licensed.");
			Console.WriteLine("Usage: [-file filename] [arg1] [arg2] ...");
			Console.WriteLine();

			var script = new ScriptInterpreter();

			script.AddMethodSources(new FileMethods());
			script.AddMethodSources(new TableMethods());
			script.AddMethodSources(new EMailMethods());
			script.AddMethodSources(new IndexMethods());

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
		}
	}
}