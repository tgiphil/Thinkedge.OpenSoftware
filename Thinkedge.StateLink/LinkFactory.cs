using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace Thinkedge.StateLink
{
	public class LinkFactory
	{
		public string CipherKey;
		public string Salt;
		public string VI;

		public LinkFactory(string key, string salt = null, string vi = null)
		{
			CipherKey = key;
			Salt = salt;
			VI = vi;

			Complete();
		}

		private void Complete()
		{
			if (Salt == null)
			{
				var sha = SHA1.Create();
				var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(CipherKey));
				Salt = Encoding.UTF8.GetString(hash, 0, hash.Length);
			}
			if (VI == null)
			{
				var sha = SHA1.Create();
				var hash = sha.ComputeHash(Encoding.ASCII.GetBytes(Salt + CipherKey));
				VI = Encoding.UTF8.GetString(hash, 0, hash.Length).Substring(0, 16);
			}
		}

		public string Encrypt(string plainText)
		{
			var sha = SHA256.Create();

			var dataBytes = Encoding.UTF8.GetBytes(plainText);

			var key = new Rfc2898DeriveBytes(CipherKey, Encoding.ASCII.GetBytes(Salt)).GetBytes(32);
			var symmetricKey = new AesManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };

			var encryptor = symmetricKey.CreateEncryptor(key, Encoding.ASCII.GetBytes(VI));

			byte[] cipher;

			using (var memoryStream = new MemoryStream())
			{
				using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
				{
					cryptoStream.Write(dataBytes, 0, dataBytes.Length);
					cryptoStream.FlushFinalBlock();
					cipher = memoryStream.ToArray();
					cryptoStream.Close();
				}
				memoryStream.Close();
			}

			var base64 = Convert.ToBase64String(cipher);

			var escaped = Uri.EscapeDataString(base64);

			return escaped;
		}

		public string Decrypt(string encryptedText)
		{
			var unescaped = Uri.UnescapeDataString(encryptedText);

			var decoded = Convert.FromBase64String(unescaped);

			var sha = SHA256.Create();
			var key = new Rfc2898DeriveBytes(CipherKey, Encoding.ASCII.GetBytes(Salt)).GetBytes(32);
			var symmetricKey = new AesManaged() { Mode = CipherMode.CBC, Padding = PaddingMode.PKCS7 };

			var decryptor = symmetricKey.CreateDecryptor(key, Encoding.ASCII.GetBytes(VI));

			var memoryStream = new MemoryStream(decoded);
			var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);
			var text = new byte[decoded.Length + 32];

			int bytes = cryptoStream.Read(text, 0, text.Length);

			memoryStream.Close();
			cryptoStream.Close();

			var plain = Encoding.UTF8.GetString(text, 0, bytes);

			return plain;
		}

		private bool CompareHash(byte[] hash1, byte[] hash2)
		{
			if (hash1.Length != hash2.Length)
				return false;

			for (int i = 0; i < hash1.Length; i++)
			{
				if (hash1[i] != hash2[i])
					return false;
			}

			return true;
		}
	}
}