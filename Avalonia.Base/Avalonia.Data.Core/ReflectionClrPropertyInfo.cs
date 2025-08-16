using System;
using System.Linq.Expressions;
using System.Reflection;

namespace Avalonia.Data.Core;

public class ReflectionClrPropertyInfo : ClrPropertyInfo
{
	private static Action<object, object?>? CreateSetter(PropertyInfo info)
	{
		if (info.SetMethod == null)
		{
			return null;
		}
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "target");
		ParameterExpression parameterExpression2 = Expression.Parameter(typeof(object), "value");
		return Expression.Lambda<Action<object, object>>(Expression.Call(Expression.Convert(parameterExpression, info.DeclaringType), info.SetMethod, Expression.Convert(parameterExpression2, info.SetMethod.GetParameters()[0].ParameterType)), new ParameterExpression[2] { parameterExpression, parameterExpression2 }).Compile();
	}

	private static Func<object, object>? CreateGetter(PropertyInfo info)
	{
		if (info.GetMethod == null)
		{
			return null;
		}
		ParameterExpression parameterExpression = Expression.Parameter(typeof(object), "target");
		return Expression.Lambda<Func<object, object>>(Expression.Convert(Expression.Call(Expression.Convert(parameterExpression, info.DeclaringType), info.GetMethod), typeof(object)), new ParameterExpression[1] { parameterExpression }).Compile();
	}

	public ReflectionClrPropertyInfo(PropertyInfo info)
		: base(info.Name, CreateGetter(info), CreateSetter(info), info.PropertyType)
	{
	}
}
