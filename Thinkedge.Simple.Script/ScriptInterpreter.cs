using System.Collections.Generic;
using Thinkedge.Common;
using Thinkedge.Simple.Evaluator;

namespace Thinkedge.Simple.Script
{
	public class ScriptInterpreter : BaseStandardResult, IMethodSource, IVariableSource
	{
		protected Dictionary<string, Value> Variables = new Dictionary<string, Value>();
		protected IList<IMethodSource> MethodSources = new List<IMethodSource>();

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
			var expressionText = string.Empty;

			int posEqual = command.IndexOf('=');

			if (posEqual > 0)
			{
				expressionText = command.Substring(posEqual + 1);
				variable = command.Substring(0, posEqual - 1);
			}
			else
			{
				variable = string.Empty;
				expressionText = command;
			}

			var expression = ExpressionCache.Compile(expressionText);

			if (!expression.IsValid)
				return ReturnError<Value>("ValidateTable() error: occurred during evaluating: " + expression.Parser.Tokenizer.Expression);

			var result = expression.Evaluate(new Context() { MethodSource = this, VariableSource = this });

			if (result.IsError)
				return ReturnError<Value>(result.String);

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

		Value IMethodSource.Evaluate(string name, IList<Value> parameters, Context context)
		{
			foreach (var methodSource in MethodSources)
			{
				var result = methodSource.Evaluate(name, parameters, context);

				if (result == null)
					continue;

				return result;
			}

			return null;
		}
	}
}