using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;
using Thinkedge.Simple.ExpressionEngine;
using Thinkedge.Simple.Table;

namespace Thinkedge.TableScript.Method
{
	public class SharePointMethods : IMethodSource
	{
		Value IMethodSource.Evaluate(string name, IList<Value> parameters, Context context)
		{
			switch (name)
			{
				//short versions
				case "GetAllUsers": return GetAllUsers(parameters);
				// long versions
				case "SharePoint.GetAllUsers": return GetAllUsers(parameters);
				default: break;
			}

			return null;
		}

		public static Value GetAllUsers(IList<Value> parameters)
		{
			var validate = Expression.ValidateHelper("GetAllUsers", parameters, 1, new List<ValueType>() { ValueType.String });

			if (validate != null)
				return validate;

			if (parameters.Count == 5 && !parameters[4].IsString)
				return Value.CreateErrorValue("parameter #5 is not a string");

			var site = parameters[0].String;

			try
			{
				var result = GetAllUsersTable(site);

				return new Value(result);
			}
			catch (System.Exception e)
			{
				return Value.CreateErrorValue("unable to perform lookup update", e);
			}
		}

		private static SimpleTable GetAllUsersTable(string site)
		{
			var people = GetAllUsers(site);

			var table = new SimpleTable();
			table.AddColumnName("ID");
			table.AddColumnName("Name");
			table.AddColumnName("EMail");

			foreach (var person in people)
			{
				var row = table.CreateRow();

				row.SetField("ID", person.id);
				row.SetField("Name", person.Title);
				row.SetField("EMail", person.Email);
			}

			return table;
		}

		private static List<People> GetAllUsers(string site)
		{
			var posts = new List<People>();
			var request = (HttpWebRequest)HttpWebRequest.Create(site + "/_api/web/siteusers");
			request.Method = "GET";
			request.Accept = "application/json;odata=verbose";
			request.ContentType = "application/json;odata=verbose";
			request.Credentials = CredentialCache.DefaultCredentials;
			var response = request.GetResponse();
			Data data = null;

			using (response)
			{
				using (var reader = new StreamReader(response.GetResponseStream()))
				{
					var serializer = new JavaScriptSerializer();

					string jSON = reader.ReadToEnd();
					data = serializer.Deserialize<Data>(jSON);
				}
			}

			foreach (var post in data.d.results)
			{
				posts.Add(post);
			}

			return posts;
		}

		#region SharePoint Data

		private class Data
		{
			public Results d { get; set; }
		}

		private class Results
		{
			public People[] results { get; set; }
		}

		private class People
		{
			public string id { get; set; }
			public string Title { get; set; }
			public string Email { get; set; }
		}

		#endregion SharePoint Data
	}
}