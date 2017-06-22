using System.Collections.Generic;

namespace Thinkedge.Simple.ExpressionEngine
{
	public class ExpressionNode
	{
		public Token Token { get; protected set; }

		public ExpressionNode Left { get; protected set; } = null;
		public ExpressionNode Right { get; protected set; } = null;

		public List<ExpressionNode> Parameters { get; protected set; } = null;

		public ExpressionNode(Token token)
		{
			Token = token;
		}

		public ExpressionNode(Token token, ExpressionNode left)
		{
			Token = token;
			Left = left;
		}

		public ExpressionNode(Token token, ExpressionNode left, ExpressionNode right)
		{
			Token = token;
			Left = left;
			Right = right;
		}

		public ExpressionNode(Token token, List<ExpressionNode> parameters)
		{
			Token = token;
			Parameters = parameters;
		}

		public override string ToString()
		{
			return Token.ToString();
		}
	}
}