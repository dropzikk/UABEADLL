using System;
using System.Linq.Expressions;

namespace Metsys.Bson.Configuration;

public static class ExpressionHelper
{
	private class ExpressionNameVisitor
	{
		public string Visit(Expression expression)
		{
			if (expression is UnaryExpression)
			{
				expression = ((UnaryExpression)expression).Operand;
			}
			if (expression is MethodCallExpression)
			{
				return Visit((MethodCallExpression)expression);
			}
			if (expression is MemberExpression)
			{
				return Visit((MemberExpression)expression);
			}
			if (expression is BinaryExpression && expression.NodeType == ExpressionType.ArrayIndex)
			{
				return Visit((BinaryExpression)expression);
			}
			return null;
		}

		private string Visit(BinaryExpression expression)
		{
			string text = null;
			if (expression.Left is MemberExpression)
			{
				text = Visit((MemberExpression)expression.Left);
			}
			object value = Expression.Lambda(expression.Right).Compile().DynamicInvoke();
			return text + $"[{value}]";
		}

		private string Visit(MemberExpression expression)
		{
			string text = expression.Member.Name;
			string text2 = Visit(expression.Expression);
			if (text2 != null)
			{
				text = text2 + "." + text;
			}
			return text;
		}

		private string Visit(MethodCallExpression expression)
		{
			string text = null;
			if (expression.Object is MemberExpression)
			{
				text = Visit((MemberExpression)expression.Object);
			}
			if (expression.Method.Name == "get_Item" && expression.Arguments.Count == 1)
			{
				object value = Expression.Lambda(expression.Arguments[0]).Compile().DynamicInvoke();
				text += $"[{value}]";
			}
			return text;
		}
	}

	public static string GetName(this MemberExpression expression)
	{
		return new ExpressionNameVisitor().Visit(expression);
	}

	public static MemberExpression GetMemberExpression<T, TValue>(this Expression<Func<T, TValue>> expression)
	{
		if (expression == null)
		{
			return null;
		}
		if (expression.Body is MemberExpression)
		{
			return (MemberExpression)expression.Body;
		}
		if (expression.Body is UnaryExpression)
		{
			Expression operand = ((UnaryExpression)expression.Body).Operand;
			if (operand is MemberExpression)
			{
				return (MemberExpression)operand;
			}
			if (operand is MethodCallExpression)
			{
				return ((MethodCallExpression)operand).Object as MemberExpression;
			}
		}
		return null;
	}
}
