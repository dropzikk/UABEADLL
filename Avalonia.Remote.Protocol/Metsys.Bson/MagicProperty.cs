using System;
using System.Reflection;

namespace Metsys.Bson;

internal class MagicProperty
{
	private readonly PropertyInfo _property;

	private readonly string _name;

	private readonly bool _ignored;

	public readonly bool _ignoredIfNull;

	public Type Type => _property.PropertyType;

	public string Name => _name;

	public bool Ignored => _ignored;

	public bool IgnoredIfNull => _ignoredIfNull;

	public Action<object, object> Setter { get; private set; }

	public Func<object, object> Getter { get; private set; }

	public MagicProperty(PropertyInfo property, string name, bool ignored, bool ignoredIfNull)
	{
		_property = property;
		_name = name;
		_ignored = ignored;
		_ignoredIfNull = ignoredIfNull;
		Getter = CreateGetterMethod(property);
		Setter = CreateSetterMethod(property);
	}

	private static Action<object, object> CreateSetterMethod(PropertyInfo property)
	{
		return (Action<object, object>)typeof(MagicProperty).GetMethod("SetterMethod", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.DeclaringType, property.PropertyType).Invoke(null, new object[1] { property });
	}

	private static Func<object, object> CreateGetterMethod(PropertyInfo property)
	{
		return (Func<object, object>)typeof(MagicProperty).GetMethod("GetterMethod", BindingFlags.Static | BindingFlags.NonPublic).MakeGenericMethod(property.DeclaringType, property.PropertyType).Invoke(null, new object[1] { property });
	}

	private static Action<object, object> SetterMethod<TTarget, TParam>(PropertyInfo method) where TTarget : class
	{
		MethodInfo setMethod = method.GetSetMethod(nonPublic: true);
		if (setMethod == null)
		{
			return null;
		}
		Action<TTarget, TParam> func = (Action<TTarget, TParam>)Delegate.CreateDelegate(typeof(Action<TTarget, TParam>), setMethod);
		return delegate(object target, object param)
		{
			func((TTarget)target, (TParam)param);
		};
	}

	private static Func<object, object> GetterMethod<TTarget, TParam>(PropertyInfo method) where TTarget : class
	{
		Func<TTarget, TParam> func = (Func<TTarget, TParam>)Delegate.CreateDelegate(method: method.GetGetMethod(nonPublic: true), type: typeof(Func<TTarget, TParam>));
		return (object target) => func((TTarget)target);
	}
}
