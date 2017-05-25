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

		public StandardResult<Value> Execute(string command)
		{
			ClearError();

			var result = InternalExecute(command);

			return result;
		}

		protected StandardResult<Value> InternalExecute(string command)
		{
			if (string.IsNullOrWhiteSpace(command))
				return ReturnResult<Value>(new Value(string.Empty));

			if (command.StartsWith("#"))
				return ReturnResult<Value>(new Value(string.Empty));

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
				return ReturnError<Value>("ValidateTable() error: occurred during evaluating: " + eval.Parser.Tokenizer.Expression);

			var result = eval.Evaluate(new Context() { MethodSource = this, VariableSource = this });

			if (result.IsError)
				return ReturnResult<Value>(result);

			if (!string.IsNullOrWhiteSpace(variable))
			{
				Variables.Remove(variable);
				Variables.Add(variable, result);
			}

			return ReturnResult<Value>(result);
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