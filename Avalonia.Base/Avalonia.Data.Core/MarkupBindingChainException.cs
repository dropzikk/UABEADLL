using System.Collections.Generic;
using System.Linq;

namespace Avalonia.Data.Core;

internal class MarkupBindingChainException : BindingChainException
{
	private IList<string>? _nodes = new List<string>();

	public bool HasNodes
	{
		get
		{
			IList<string>? nodes = _nodes;
			if (nodes == null)
			{
				return false;
			}
			return nodes.Count > 0;
		}
	}

	public MarkupBindingChainException(string message)
		: base(message)
	{
	}

	public MarkupBindingChainException(string message, string node)
		: base(message)
	{
		AddNode(node);
	}

	public MarkupBindingChainException(string message, string expression, string expressionNullPoint)
		: base(message, expression, expressionNullPoint)
	{
		_nodes = null;
	}

	public void AddNode(string node)
	{
		_nodes?.Add(node);
	}

	public void Commit(string expression)
	{
		base.Expression = expression;
		base.ExpressionErrorPoint = ((_nodes != null) ? string.Join(".", _nodes.Reverse()).Replace(".!", "!").Replace(".[", "[")
			.Replace(".^", "^") : string.Empty);
		_nodes = null;
	}
}
