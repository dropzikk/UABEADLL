using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;

namespace Avalonia.Data.Core.Plugins;

internal class MethodAccessorPlugin : IPropertyAccessorPlugin
{
	private sealed class Accessor : PropertyAccessorBase
	{
		public override Type? PropertyType { get; }

		public override object? Value { get; }

		public Accessor(WeakReference<object?> reference, MethodInfo method)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			if ((object)method == null)
			{
				throw new ArgumentNullException("method");
			}
			Type returnType = method.ReturnType;
			ParameterInfo[] parameters = method.GetParameters();
			Type[] array = new Type[parameters.Length + 1];
			for (int i = 0; i < parameters.Length; i++)
			{
				ParameterInfo parameterInfo = parameters[i];
				array[i] = parameterInfo.ParameterType;
			}
			array[^1] = returnType;
			PropertyType = Expression.GetDelegateType(array);
			object target;
			if (method.IsStatic)
			{
				Value = method.CreateDelegate(PropertyType);
			}
			else if (reference.TryGetTarget(out target))
			{
				Value = method.CreateDelegate(PropertyType, target);
			}
		}

		public override bool SetValue(object? value, BindingPriority priority)
		{
			return false;
		}

		protected override void SubscribeCore()
		{
			try
			{
				PublishValue(Value);
			}
			catch
			{
			}
		}

		protected override void UnsubscribeCore()
		{
		}
	}

	private readonly Dictionary<(Type, string), MethodInfo?> _methodLookup = new Dictionary<(Type, string), MethodInfo>();

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public bool Match(object obj, string methodName)
	{
		return GetFirstMethodWithName(obj.GetType(), methodName) != null;
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public IPropertyAccessor? Start(WeakReference<object?> reference, string methodName)
	{
		if (reference == null)
		{
			throw new ArgumentNullException("reference");
		}
		if (methodName == null)
		{
			throw new ArgumentNullException("methodName");
		}
		if (!reference.TryGetTarget(out object target) || target == null)
		{
			return null;
		}
		MethodInfo firstMethodWithName = GetFirstMethodWithName(target.GetType(), methodName);
		if ((object)firstMethodWithName != null)
		{
			return new Accessor(reference, firstMethodWithName);
		}
		return new PropertyError(new BindingNotification(new MissingMemberException($"Could not find CLR method '{methodName}' on '{target}'"), BindingErrorType.Error));
	}

	private MethodInfo? GetFirstMethodWithName([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type type, string methodName)
	{
		(Type, string) key = (type, methodName);
		if (!_methodLookup.TryGetValue(key, out MethodInfo value))
		{
			return TryFindAndCacheMethod(type, methodName);
		}
		return value;
	}

	private MethodInfo? TryFindAndCacheMethod([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicMethods | DynamicallyAccessedMemberTypes.NonPublicMethods)] Type type, string methodName)
	{
		MethodInfo methodInfo = null;
		MethodInfo[] methods = type.GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (MethodInfo methodInfo2 in methods)
		{
			if (methodInfo2.Name == methodName)
			{
				ParameterInfo[] parameters = methodInfo2.GetParameters();
				if (parameters.Length == 1 && parameters[0].ParameterType == typeof(object))
				{
					methodInfo = methodInfo2;
					break;
				}
				if (parameters.Length == 0)
				{
					methodInfo = methodInfo2;
				}
			}
		}
		_methodLookup.Add((type, methodName), methodInfo);
		return methodInfo;
	}
}
