using System;
using System.Collections.Generic;
using System.Text;

namespace Thinkedge.Common
{
	public class FilePackage
	{
		public List<InternalFile> Files = new List<InternalFile>();

		public DateTime Received = DateTime.Now;
		public string IPAddress;

		public void Add(InternalFile packageFile)
		{
			Files.Add(packageFile);
		}

		public InternalFile CreateManifestFile()
		{
			var sb = new StringBuilder();

			sb.AppendLine("Manifest======");
			sb.AppendLine();

			if (IPAddress != null)
				sb.AppendLine("SourceIP: " + IPAddress);

			sb.AppendLine("Received: " + Received.ToLongDateString() + " " + Received.ToLongTimeString());
			sb.AppendLine();

			foreach (var file in Files)
			{
				sb.Append(file.Filename);
				sb.Append(" [size=");
				sb.Append(file.Stream.Length);
				sb.AppendLine("]");
			}

			var packageFile = new InternalFile("_Manifest.txt", sb.ToString(), "text/plain");

			return packageFile;
		}

		public void AddManifestFile()
		{
			Add(CreateManifestFile());
		}

		public bool ValidateFileExtensions(string[] validExtensions)
		{
			foreach (var file in Files)
			{
				if (!file.ValidateFileExtension(validExtensions))
					return false;
			}

			return true;
		}

		public bool ValidateNoFilesStartWithUnderscore()
		{
			foreach (var file in Files)
			{
				if (file.ShortFilename.StartsWith("_"))
					return false;
			}

			return true;
		}
	}
}