using System.Collections.Generic;
using Thinkedge.Common;
using Thinkedge.Simple.Expression;

namespace Thinkedge.Simple.Script
{
	public class ScriptInterpreter : BaseStandardResult, IMethodSource, IVariableSource
	{
		protected Dictionary<string, Value> Variables = new Dictionary<string, Value>();
		protected IList<IMethodSource> MethodSources = new List<IMethodSource>();
		protected EvaluatorCache cache = new EvaluatorCache();

		public ScriptInterpreter()
		{
		}

		public void AddMethodSources(IMethodSource methodSource)
		{
			MethodSources.Add(methodSource);
		}

		public void SetVariableArgument(int index, string value)
		{
			Variables.Add("$" + index.ToString(), new Value(value));
		}

		public bool Execute(string command)
		{
			ClearError();

			var success = InternalExecute(command);

			return !HasError;
		}

		protected bool InternalExecute(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
				return true;

			if (command.StartsWith("#"))
				return true;

			var variable = string.Empty;
			var expression = string.Empty;

			int posEqual = command.IndexOf('=');

			if (posEqual > 0)
			{
				expression = command.Substring(posEqual + 1);
				variable = command.Substring(0, posEqual - 1);
			}
			else
			{
				variable = string.Empty;
				expression = command;
			}

			var eval = cache.Compile(expression);

			if (!eval.IsValid)
			{
				ErrorMessage = "compile error";
				return false;
			}

			var result = eval.Evaluate(this, this);

			if (result.IsError)
			{
				if (!HasError)
					ErrorMessage = result.String;

				return false;
			}

			if (!string.IsNullOrWhiteSpace(variable))
			{
				Variables.Remove(variable);
				Variables.Add(variable, result);
			}

			return true;
		}

		Value IVariableSource.GetVariable(string name)
		{
			if (Variables.ContainsKey(name))
			{
				var variable = Variables[name];
				return variable;
			}

			return null;
		}

		Value IMethodSource.Evaluate(string name, IList<Value> parameters)
		{
			foreach (var methodSource in MethodSources)
			{
				var result = methodSource.Evaluate(name, parameters);

				if (result == null)
					continue;

				return result;
			}

			return null;
		}
	}
}