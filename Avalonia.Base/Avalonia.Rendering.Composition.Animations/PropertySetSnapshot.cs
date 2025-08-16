using System.Collections.Generic;
using Avalonia.Rendering.Composition.Expressions;

namespace Avalonia.Rendering.Composition.Animations;

internal class PropertySetSnapshot : IExpressionParameterCollection, IExpressionObject
{
	public struct Value
	{
		public ExpressionVariant Variant;

		public IExpressionObject Object;

		public Value(IExpressionObject o)
		{
			Object = o;
			Variant = default(ExpressionVariant);
		}

		public static implicit operator Value(ExpressionVariant v)
		{
			Value result = default(Value);
			result.Variant = v;
			return result;
		}
	}

	private readonly Dictionary<string, Value> _dic;

	public PropertySetSnapshot(Dictionary<string, Value> dic)
	{
		_dic = dic;
	}

	public ExpressionVariant GetParameter(string name)
	{
		_dic.TryGetValue(name, out var value);
		return value.Variant;
	}

	public IExpressionObject GetObjectParameter(string name)
	{
		_dic.TryGetValue(name, out var value);
		return value.Object;
	}

	public ExpressionVariant GetProperty(string name)
	{
		return GetParameter(name);
	}
}
