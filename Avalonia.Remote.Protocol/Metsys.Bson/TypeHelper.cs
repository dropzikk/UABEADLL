using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Metsys.Bson.Configuration;

namespace Metsys.Bson;

[RequiresUnreferencedCode("Bson uses reflection")]
internal class TypeHelper
{
	private static readonly IDictionary<Type, TypeHelper> _cachedTypeLookup = new Dictionary<Type, TypeHelper>();

	private static readonly BsonConfiguration _configuration = BsonConfiguration.Instance;

	private readonly IDictionary<string, MagicProperty> _properties;

	public MagicProperty Expando { get; private set; }

	private TypeHelper(Type type)
	{
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.FlattenHierarchy);
		_properties = LoadMagicProperties(type, properties);
		if (typeof(IExpando).IsAssignableFrom(type))
		{
			Expando = _properties["Expando"];
		}
	}

	public ICollection<MagicProperty> GetProperties()
	{
		return _properties.Values;
	}

	public MagicProperty FindProperty(string name)
	{
		_properties.TryGetValue(name, out var value);
		return value;
	}

	public static TypeHelper GetHelperForType(Type type)
	{
		if (!_cachedTypeLookup.TryGetValue(type, out var value))
		{
			value = new TypeHelper(type);
			_cachedTypeLookup[type] = value;
		}
		return value;
	}

	public static string FindProperty(LambdaExpression lambdaExpression)
	{
		Expression expression = lambdaExpression;
		bool flag = false;
		while (!flag)
		{
			switch (expression.NodeType)
			{
			case ExpressionType.Convert:
				expression = ((UnaryExpression)expression).Operand;
				break;
			case ExpressionType.Lambda:
				expression = ((LambdaExpression)expression).Body;
				break;
			case ExpressionType.MemberAccess:
			{
				MemberExpression memberExpression = (MemberExpression)expression;
				if (memberExpression.Expression.NodeType != ExpressionType.Parameter && memberExpression.Expression.NodeType != ExpressionType.Convert)
				{
					throw new ArgumentException($"Expression '{lambdaExpression}' must resolve to top-level member.", "lambdaExpression");
				}
				return memberExpression.Member.Name;
			}
			default:
				flag = true;
				break;
			}
		}
		return null;
	}

	public static PropertyInfo FindProperty(Type type, string name)
	{
		return (from p in type.GetProperties()
			where p.Name == name
			select p).First();
	}

	private static IDictionary<string, MagicProperty> LoadMagicProperties(Type type, IEnumerable<PropertyInfo> properties)
	{
		Dictionary<string, MagicProperty> dictionary = new Dictionary<string, MagicProperty>(StringComparer.CurrentCultureIgnoreCase);
		foreach (PropertyInfo property in properties)
		{
			if (property.GetIndexParameters().Length == 0)
			{
				string text = _configuration.AliasFor(type, property.Name);
				bool ignored = _configuration.IsIgnored(type, property.Name);
				bool ignoredIfNull = _configuration.IsIgnoredIfNull(type, property.Name);
				dictionary.Add(text, new MagicProperty(property, text, ignored, ignoredIfNull));
			}
		}
		return dictionary;
	}
}
