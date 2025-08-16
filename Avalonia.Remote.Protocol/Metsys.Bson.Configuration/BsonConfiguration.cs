using System;
using System.Collections.Generic;

namespace Metsys.Bson.Configuration;

internal class BsonConfiguration
{
	private readonly IDictionary<Type, IDictionary<string, string>> _aliasMap = new Dictionary<Type, IDictionary<string, string>>();

	private readonly IDictionary<Type, HashSet<string>> _ignored = new Dictionary<Type, HashSet<string>>();

	private readonly IDictionary<Type, HashSet<string>> _ignoredIfNull = new Dictionary<Type, HashSet<string>>();

	private static BsonConfiguration _instance;

	internal static BsonConfiguration Instance
	{
		get
		{
			if (_instance == null)
			{
				_instance = new BsonConfiguration();
			}
			return _instance;
		}
	}

	private BsonConfiguration()
	{
	}

	public static void ForType<T>(Action<ITypeConfiguration<T>> action)
	{
		action(new TypeConfiguration<T>(Instance));
	}

	internal void AddMap<T>(string property, string alias)
	{
		Type typeFromHandle = typeof(T);
		if (!_aliasMap.ContainsKey(typeFromHandle))
		{
			_aliasMap[typeFromHandle] = new Dictionary<string, string>();
		}
		_aliasMap[typeFromHandle][property] = alias;
	}

	internal string AliasFor(Type type, string property)
	{
		if (!_aliasMap.TryGetValue(type, out var value))
		{
			return property;
		}
		if (!value.TryGetValue(property, out var value2))
		{
			return property;
		}
		return value2;
	}

	public void AddIgnore<T>(string name)
	{
		Type typeFromHandle = typeof(T);
		if (!_ignored.ContainsKey(typeFromHandle))
		{
			_ignored[typeFromHandle] = new HashSet<string>();
		}
		_ignored[typeFromHandle].Add(name);
	}

	public bool IsIgnored(Type type, string name)
	{
		if (_ignored.TryGetValue(type, out var value))
		{
			return value.Contains(name);
		}
		return false;
	}

	public void AddIgnoreIfNull<T>(string name)
	{
		Type typeFromHandle = typeof(T);
		if (!_ignoredIfNull.ContainsKey(typeFromHandle))
		{
			_ignoredIfNull[typeFromHandle] = new HashSet<string>();
		}
		_ignoredIfNull[typeFromHandle].Add(name);
	}

	public bool IsIgnoredIfNull(Type type, string name)
	{
		if (_ignoredIfNull.TryGetValue(type, out var value))
		{
			return value.Contains(name);
		}
		return false;
	}
}
