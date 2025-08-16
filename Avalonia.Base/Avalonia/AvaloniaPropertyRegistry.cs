using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Avalonia;

public class AvaloniaPropertyRegistry
{
	private readonly Dictionary<int, AvaloniaProperty> _properties = new Dictionary<int, AvaloniaProperty>();

	private readonly Dictionary<Type, Dictionary<int, AvaloniaProperty>> _registered = new Dictionary<Type, Dictionary<int, AvaloniaProperty>>();

	private readonly Dictionary<Type, Dictionary<int, AvaloniaProperty>> _attached = new Dictionary<Type, Dictionary<int, AvaloniaProperty>>();

	private readonly Dictionary<Type, Dictionary<int, AvaloniaProperty>> _direct = new Dictionary<Type, Dictionary<int, AvaloniaProperty>>();

	private readonly Dictionary<Type, List<AvaloniaProperty>> _registeredCache = new Dictionary<Type, List<AvaloniaProperty>>();

	private readonly Dictionary<Type, List<AvaloniaProperty>> _attachedCache = new Dictionary<Type, List<AvaloniaProperty>>();

	private readonly Dictionary<Type, List<AvaloniaProperty>> _directCache = new Dictionary<Type, List<AvaloniaProperty>>();

	private readonly Dictionary<Type, List<AvaloniaProperty>> _inheritedCache = new Dictionary<Type, List<AvaloniaProperty>>();

	public static AvaloniaPropertyRegistry Instance { get; } = new AvaloniaPropertyRegistry();

	internal IReadOnlyCollection<AvaloniaProperty> Properties => _properties.Values;

	[UnconditionalSuppressMessage("Trimming", "IL2059", Justification = "If type was trimmed out, no properties were referenced")]
	public IReadOnlyList<AvaloniaProperty> GetRegistered(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_registeredCache.TryGetValue(type, out List<AvaloniaProperty> value))
		{
			return value;
		}
		Type type2 = type;
		value = new List<AvaloniaProperty>();
		while (type2 != null)
		{
			RuntimeHelpers.RunClassConstructor(type2.TypeHandle);
			if (_registered.TryGetValue(type2, out Dictionary<int, AvaloniaProperty> value2))
			{
				value.AddRange(value2.Values);
			}
			type2 = type2.BaseType;
		}
		_registeredCache.Add(type, value);
		return value;
	}

	public IReadOnlyList<AvaloniaProperty> GetRegisteredAttached(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_attachedCache.TryGetValue(type, out List<AvaloniaProperty> value))
		{
			return value;
		}
		Type type2 = type;
		value = new List<AvaloniaProperty>();
		while (type2 != null)
		{
			if (_attached.TryGetValue(type2, out Dictionary<int, AvaloniaProperty> value2))
			{
				value.AddRange(value2.Values);
			}
			type2 = type2.BaseType;
		}
		_attachedCache.Add(type, value);
		return value;
	}

	public IReadOnlyList<AvaloniaProperty> GetRegisteredDirect(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_directCache.TryGetValue(type, out List<AvaloniaProperty> value))
		{
			return value;
		}
		Type type2 = type;
		value = new List<AvaloniaProperty>();
		while (type2 != null)
		{
			if (_direct.TryGetValue(type2, out Dictionary<int, AvaloniaProperty> value2))
			{
				value.AddRange(value2.Values);
			}
			type2 = type2.BaseType;
		}
		_directCache.Add(type, value);
		return value;
	}

	public IReadOnlyList<AvaloniaProperty> GetRegisteredInherited(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_inheritedCache.TryGetValue(type, out List<AvaloniaProperty> value))
		{
			return value;
		}
		value = new List<AvaloniaProperty>();
		HashSet<AvaloniaProperty> hashSet = new HashSet<AvaloniaProperty>();
		IReadOnlyList<AvaloniaProperty> registered = GetRegistered(type);
		int count = registered.Count;
		for (int i = 0; i < count; i++)
		{
			AvaloniaProperty avaloniaProperty = registered[i];
			if (avaloniaProperty.Inherits)
			{
				value.Add(avaloniaProperty);
				hashSet.Add(avaloniaProperty);
			}
		}
		IReadOnlyList<AvaloniaProperty> registeredAttached = GetRegisteredAttached(type);
		int count2 = registeredAttached.Count;
		for (int j = 0; j < count2; j++)
		{
			AvaloniaProperty avaloniaProperty2 = registeredAttached[j];
			if (avaloniaProperty2.Inherits && !hashSet.Contains(avaloniaProperty2))
			{
				value.Add(avaloniaProperty2);
			}
		}
		_inheritedCache.Add(type, value);
		return value;
	}

	public IReadOnlyList<AvaloniaProperty> GetRegistered(AvaloniaObject o)
	{
		if (o == null)
		{
			throw new ArgumentNullException("o");
		}
		return GetRegistered(o.GetType());
	}

	public DirectPropertyBase<T> GetRegisteredDirect<T>(AvaloniaObject o, DirectPropertyBase<T> property)
	{
		return FindRegisteredDirect(o, property) ?? throw new ArgumentException($"Property '{property.Name} not registered on '{o.GetType()}");
	}

	public AvaloniaProperty? FindRegistered(Type type, string name)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Contains('.'))
		{
			throw new InvalidOperationException("Attached properties not supported.");
		}
		IReadOnlyList<AvaloniaProperty> registered = GetRegistered(type);
		int count = registered.Count;
		for (int i = 0; i < count; i++)
		{
			AvaloniaProperty avaloniaProperty = registered[i];
			if (avaloniaProperty.Name == name)
			{
				return avaloniaProperty;
			}
		}
		return null;
	}

	public AvaloniaProperty? FindRegistered(AvaloniaObject o, string name)
	{
		if (o == null)
		{
			throw new ArgumentNullException("o");
		}
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		return FindRegistered(o.GetType(), name);
	}

	public DirectPropertyBase<T>? FindRegisteredDirect<T>(AvaloniaObject o, DirectPropertyBase<T> property)
	{
		if (property.Owner == o.GetType())
		{
			return property;
		}
		IReadOnlyList<AvaloniaProperty> registeredDirect = GetRegisteredDirect(o.GetType());
		int count = registeredDirect.Count;
		for (int i = 0; i < count; i++)
		{
			AvaloniaProperty avaloniaProperty = registeredDirect[i];
			if (avaloniaProperty == property)
			{
				return (DirectPropertyBase<T>)avaloniaProperty;
			}
		}
		return null;
	}

	internal AvaloniaProperty? FindRegistered(int id)
	{
		if (id >= _properties.Count)
		{
			return null;
		}
		return _properties[id];
	}

	public bool IsRegistered(Type type, AvaloniaProperty property)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		if (!ContainsProperty(Instance.GetRegistered(type), property))
		{
			return ContainsProperty(Instance.GetRegisteredAttached(type), property);
		}
		return true;
		static bool ContainsProperty(IReadOnlyList<AvaloniaProperty> properties, AvaloniaProperty property)
		{
			int count = properties.Count;
			for (int i = 0; i < count; i++)
			{
				if (properties[i] == property)
				{
					return true;
				}
			}
			return false;
		}
	}

	public bool IsRegistered(object o, AvaloniaProperty property)
	{
		if (o == null)
		{
			throw new ArgumentNullException("o");
		}
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		return IsRegistered(o.GetType(), property);
	}

	public void Register(Type type, AvaloniaProperty property)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		if (!_registered.TryGetValue(type, out Dictionary<int, AvaloniaProperty> value))
		{
			value = new Dictionary<int, AvaloniaProperty>();
			value.Add(property.Id, property);
			_registered.Add(type, value);
		}
		else if (!value.ContainsKey(property.Id))
		{
			value.Add(property.Id, property);
		}
		if (property.IsDirect)
		{
			if (!_direct.TryGetValue(type, out value))
			{
				value = new Dictionary<int, AvaloniaProperty>();
				value.Add(property.Id, property);
				_direct.Add(type, value);
			}
			else if (!value.ContainsKey(property.Id))
			{
				value.Add(property.Id, property);
			}
			_directCache.Clear();
		}
		if (!_properties.ContainsKey(property.Id))
		{
			_properties.Add(property.Id, property);
		}
		_registeredCache.Clear();
		_inheritedCache.Clear();
	}

	public void RegisterAttached(Type type, AvaloniaProperty property)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if ((object)property == null)
		{
			throw new ArgumentNullException("property");
		}
		if (!property.IsAttached)
		{
			throw new InvalidOperationException("Cannot register a non-attached property as attached.");
		}
		if (!_attached.TryGetValue(type, out Dictionary<int, AvaloniaProperty> value))
		{
			value = new Dictionary<int, AvaloniaProperty>();
			value.Add(property.Id, property);
			_attached.Add(type, value);
		}
		else
		{
			value.Add(property.Id, property);
		}
		_attachedCache.Clear();
		_inheritedCache.Clear();
	}
}
