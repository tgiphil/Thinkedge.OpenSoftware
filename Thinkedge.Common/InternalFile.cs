using System.IO;
using System.Linq;
using System.Text;

namespace Thinkedge.Common
{
	public class InternalFile
	{
		public string Filename { get; set; }
		public Stream Stream { get; set; }
		public string ContentType { get; set; }

		public string ShortFilename { get { return Path.GetFileName(Filename); } }
		public long Length { get { return Stream.Length; } }

		public InternalFile(string filename, Stream stream, string contentType)
		{
			Filename = filename;
			Stream = stream;
			ContentType = contentType;
		}

		public InternalFile(string filename, Stream stream) : this(filename, stream, null)
		{
		}

		public InternalFile(string filename, string data, string contentType)
		{
			var stream = new MemoryStream(Encoding.UTF8.GetBytes(data));

			Filename = filename;
			Stream = stream;
			ContentType = contentType;
		}

		public InternalFile(string filename, string data) : this(filename, data, null)
		{
		}

		public bool ValidateFileExtension(string[] validExtensions)
		{
			string ext = Path.GetExtension(Filename).ToLower();

			return validExtensions.Contains(ext);
		}
	}
}