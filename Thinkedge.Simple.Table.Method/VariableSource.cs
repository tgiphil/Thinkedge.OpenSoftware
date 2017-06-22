using System.Collections.Generic;
using Thinkedge.Simple.ExpressionEngine;

namespace Thinkedge.Simple.Table.Method
{
	public class VariableSource : IVariableSource
	{
		protected Dictionary<string, string> Variables = new Dictionary<string, string>();

		public VariableSource()
		{
		}

		public void SetVariable(string name, string value)
		{
			Variables.Remove(name);
			Variables.Add(name, value);
		}

		public void ClearVariables()
		{
			Variables.Clear();
		}

		Value IVariableSource.GetVariable(string name)
		{
			if (Variables.ContainsKey(name))
				return new Value(Variables[name]);

			return null;
		}
	}
}