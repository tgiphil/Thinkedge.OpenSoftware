using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Web;

namespace Thinkedge.StateLink
{
	public class LinkValues
	{
		protected Dictionary<string, string> values = new Dictionary<string, string>();

		public string this[string value] { get { return Get(value); } }

		public LinkValues()
		{
		}

		public LinkValues(string encoded)
		{
			Decode(encoded);
		}

		public void Add(string key, string value)
		{
			values.Remove(key);

			if (value != null)
				values.Add(key, value);
		}

		public string Get(string key)
		{
			string value = null;

			values.TryGetValue(key, out value);

			return value;
		}

		public string Encode()
		{
			var nvc = new NameValueCollection();

			foreach (var kvp in values)
			{
				nvc.Add(kvp.Key.ToString(), kvp.Value.ToString());
			}

			var s = String.Join("&", nvc.AllKeys.Select(a => a + "=" + HttpUtility.UrlEncode(nvc[a])));

			return s;
		}

		public void Decode(string source)
		{
			var vc = HttpUtility.ParseQueryString(source);

			foreach (var k in vc.AllKeys)
			{
				var v = vc.GetValues(k);

				if (v.Length == 0)
					continue;

				Add(k, v[0]);
			}
		}
	}
}