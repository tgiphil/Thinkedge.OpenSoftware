using System.Collections.Generic;
using System.Linq;
using System.Text;

// BSD License
// Derived from:
// https://github.com/poulfoged/SentenceCompression

namespace Thinkedge.TextCompression
{
	public class SmallTextCompression
	{
		private const char SingleVerbatimCharMarker = (char)254;
		private const char MultiVerbatimCharMarker = (char)255;

		private static readonly string[] PartDictionary = BuildDictionary();

		private static string[] BuildDictionary()
		{
			List<string> dict = new List<string>();

			string letters = "etaoinshrdlcumwfgypbvkjxqz";
			string symbols1 = "=&";
			string symbols2 = " .@";

			string[] common1 = new string[] { "&CN=", "&CE=", "&ED=", "&SN=", "&CI=", "&IE=", "&IL=" };
			string[] common2 = new string[] { "http", "Inc", "INC", "THE" };

			foreach (var l in letters + symbols1 + (letters.ToUpper()) + symbols2)
			{
				dict.Add(l.ToString());
			}

			foreach (var c in common1)
			{
				dict.Add(c.ToString());
			}

			foreach (var c in common2)
			{
				dict.Add(c.ToString());
			}

			return dict.ToArray();
		}

		public static string Decompress(string input)
		{
			var outputBuilder = new StringBuilder();
			var inputBuilder = new StringBuilder(input);

			while (inputBuilder.Length > 0)
			{
				switch (inputBuilder.FirstChar())
				{
					case MultiVerbatimCharMarker:
						inputBuilder.PopChar();
						var length = inputBuilder.PopChar();
						outputBuilder.AppendAndPopCharFrom(inputBuilder, length);
						break;

					case SingleVerbatimCharMarker:
						inputBuilder.PopChar();
						outputBuilder.AppendAndPopCharFrom(inputBuilder);
						break;

					default:
						outputBuilder.Append(PartDictionary[inputBuilder.PopChar()]);
						break;
				}
			}

			return outputBuilder.ToString();
		}

		public static string Compress(string input)
		{
			var outputBuilder = new StringBuilder();
			var inputBuilder = new StringBuilder(input);
			var verbatimBuilder = new StringBuilder();

			while (inputBuilder.Length > 0)
			{
				var possibleParts = PartDictionary
					.Where(p => inputBuilder.ToString().StartsWith(p))
					.ToList();

				if (verbatimBuilder.Length == char.MaxValue)
					FlushVerbatim(outputBuilder, verbatimBuilder);

				if (!possibleParts.Any())
				{
					verbatimBuilder.AppendAndPopCharFrom(inputBuilder);
					continue;
				}

				FlushVerbatim(outputBuilder, verbatimBuilder);
				var bestFit = possibleParts.OrderByDescending(p => p.Length).First();
				outputBuilder.Append((char)PartDictionary.ToList().IndexOf(bestFit));
				inputBuilder.Remove(0, bestFit.Length);
			}

			FlushVerbatim(outputBuilder, verbatimBuilder);
			return outputBuilder.ToString();
		}

		private static void FlushVerbatim(StringBuilder outputBuilder, StringBuilder verbatimBuilder)
		{
			if (verbatimBuilder.Length == 1)
			{
				outputBuilder
					.Append(SingleVerbatimCharMarker)
					.AppendAndPopCharFrom(verbatimBuilder);

				return;
			}

			if (verbatimBuilder.Length > 1)
			{
				outputBuilder
					.Append(MultiVerbatimCharMarker)
					.Append((char)verbatimBuilder.Length)
					.AppendAndPopCharFrom(verbatimBuilder, verbatimBuilder.Length);
			}
		}
	}
}