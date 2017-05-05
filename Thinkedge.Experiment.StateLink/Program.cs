using System;
using Thinkedge.StateLink;
using Thinkedge.StateLink.Configuration;

namespace ConsoleExperiment1
{
	internal class Program
	{
		private static void Main(string[] args)
		{
			string d = "A=Company Name&B=TEST2&C=TEST3&E=Phil.Garcia";

			//string ue = "hGsx5RHV0Sil225JToftDwP6fEP%2FXRRuO9ckVWDrqw1%2BIjkFV%2BU4JA3M7u%2FGSrFt74%2FaBzpuQNyYMzGzCqdbQ3pV6tXeS8gu7LB8QfXK0IE%3D";

			Console.WriteLine(d);

			var linkFactory = new LinkFactory(StateLinkConfiguration.CipherKey, StateLinkConfiguration.Salt, StateLinkConfiguration.VI);

			var e = linkFactory.Encrypt(d);
			Console.WriteLine(e);

			var o = linkFactory.Decrypt(e);
			Console.WriteLine(o);

			//var uo = linkFactory.Decrypt(ue);
			//Console.WriteLine(uo);

			LinkValues values = new LinkValues(d);

			values.Add("D", "Apple=10&70");
			Console.WriteLine(values.Get("A"));
			Console.WriteLine(values.Get("B"));
			Console.WriteLine(values.Get("C"));
			Console.WriteLine(values.Get("E"));
			Console.WriteLine(values.Get("D"));

			string vencoded = values.Encode();

			LinkValues values2 = new LinkValues();
			values2.Decode(vencoded);

			Console.WriteLine(values2.Get("A"));
			Console.WriteLine(values2.Get("B"));
			Console.WriteLine(values2.Get("C"));
			Console.WriteLine(values2.Get("E"));
			Console.WriteLine(values2.Get("D"));

			return;
		}
	}
}