using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;

namespace Avalonia.Data.Core.Parsers;

internal static class ExpressionTreeParser
{
	[RequiresUnreferencedCode("ExpressionNode might require unreferenced code.")]
	public static ExpressionNode Parse(Expression expr, bool enableDataValidation)
	{
		ExpressionVisitorNodeBuilder expressionVisitorNodeBuilder = new ExpressionVisitorNodeBuilder(enableDataValidation);
		expressionVisitorNodeBuilder.Visit(expr);
		List<ExpressionNode> nodes = expressionVisitorNodeBuilder.Nodes;
		for (int i = 0; i < nodes.Count - 1; i++)
		{
			nodes[i].Next = nodes[i + 1];
		}
		return nodes.FirstOrDefault() ?? new EmptyExpressionNode();
	}
}
