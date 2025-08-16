using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.PropertyStore;
using Avalonia.Reactive;
using Avalonia.Utilities;

namespace Avalonia;

public abstract class AvaloniaProperty : IEquatable<AvaloniaProperty>, IPropertyInfo
{
	public static readonly object UnsetValue = new UnsetValueType();

	private static int s_nextId;

	private readonly AvaloniaPropertyMetadata _defaultMetadata;

	private KeyValuePair<Type, AvaloniaPropertyMetadata>? _singleMetadata;

	private readonly Dictionary<Type, AvaloniaPropertyMetadata> _metadata;

	private readonly Dictionary<Type, AvaloniaPropertyMetadata> _metadataCache = new Dictionary<Type, AvaloniaPropertyMetadata>();

	public string Name { get; }

	public Type PropertyType { get; }

	public Type OwnerType { get; }

	public bool Inherits { get; private protected set; }

	public bool IsAttached { get; private protected set; }

	public bool IsDirect { get; private protected set; }

	public bool IsReadOnly { get; private protected set; }

	public IObservable<AvaloniaPropertyChangedEventArgs> Changed => GetChanged();

	internal Action<AvaloniaObject, bool>? Notifying { get; }

	internal int Id { get; }

	bool IPropertyInfo.CanGet => true;

	bool IPropertyInfo.CanSet => !IsReadOnly;

	private protected AvaloniaProperty(string name, Type valueType, Type ownerType, Type hostType, AvaloniaPropertyMetadata metadata, Action<AvaloniaObject, bool>? notifying = null)
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (name.Contains('.'))
		{
			throw new ArgumentException("'name' may not contain periods.");
		}
		_metadata = new Dictionary<Type, AvaloniaPropertyMetadata>();
		Name = name;
		PropertyType = valueType ?? throw new ArgumentNullException("valueType");
		OwnerType = ownerType ?? throw new ArgumentNullException("ownerType");
		Notifying = notifying;
		Id = s_nextId++;
		_metadata.Add(hostType, metadata ?? throw new ArgumentNullException("metadata"));
		_defaultMetadata = metadata.GenerateTypeSafeMetadata();
		_singleMetadata = new KeyValuePair<Type, AvaloniaPropertyMetadata>(hostType, metadata);
	}

	private protected AvaloniaProperty(AvaloniaProperty source, Type ownerType, AvaloniaPropertyMetadata? metadata)
	{
		_metadata = new Dictionary<Type, AvaloniaPropertyMetadata>();
		Name = source?.Name ?? throw new ArgumentNullException("source");
		PropertyType = source.PropertyType;
		OwnerType = ownerType ?? throw new ArgumentNullException("ownerType");
		Notifying = source.Notifying;
		Id = source.Id;
		_defaultMetadata = source._defaultMetadata;
		if (metadata != null)
		{
			_metadata.Add(ownerType, metadata);
		}
	}

	public static IndexerDescriptor operator !(AvaloniaProperty property)
	{
		return new IndexerDescriptor
		{
			Priority = BindingPriority.LocalValue,
			Property = property
		};
	}

	public static IndexerDescriptor operator ~(AvaloniaProperty property)
	{
		return new IndexerDescriptor
		{
			Priority = BindingPriority.Template,
			Property = property
		};
	}

	public static bool operator ==(AvaloniaProperty? a, AvaloniaProperty? b)
	{
		if ((object)a == b)
		{
			return true;
		}
		if ((object)a == null || (object)b == null)
		{
			return false;
		}
		return a.Equals(b);
	}

	public static bool operator !=(AvaloniaProperty? a, AvaloniaProperty? b)
	{
		return !(a == b);
	}

	public static StyledProperty<TValue> Register<TOwner, TValue>(string name, TValue defaultValue = default(TValue), bool inherits = false, BindingMode defaultBindingMode = BindingMode.OneWay, Func<TValue, bool>? validate = null, Func<AvaloniaObject, TValue, TValue>? coerce = null, bool enableDataValidation = false) where TOwner : AvaloniaObject
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		StyledPropertyMetadata<TValue> metadata = new StyledPropertyMetadata<TValue>(defaultValue, defaultBindingMode, coerce, enableDataValidation);
		StyledProperty<TValue> styledProperty = new StyledProperty<TValue>(name, typeof(TOwner), typeof(TOwner), metadata, inherits, validate);
		AvaloniaPropertyRegistry.Instance.Register(typeof(TOwner), styledProperty);
		return styledProperty;
	}

	internal static StyledProperty<TValue> Register<TOwner, TValue>(string name, TValue defaultValue, bool inherits, BindingMode defaultBindingMode, Func<TValue, bool>? validate, Func<AvaloniaObject, TValue, TValue>? coerce, bool enableDataValidation, Action<AvaloniaObject, bool>? notifying) where TOwner : AvaloniaObject
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		StyledPropertyMetadata<TValue> metadata = new StyledPropertyMetadata<TValue>(defaultValue, defaultBindingMode, coerce, enableDataValidation);
		StyledProperty<TValue> styledProperty = new StyledProperty<TValue>(name, typeof(TOwner), typeof(TOwner), metadata, inherits, validate, notifying);
		AvaloniaPropertyRegistry.Instance.Register(typeof(TOwner), styledProperty);
		return styledProperty;
	}

	public static AttachedProperty<TValue> RegisterAttached<TOwner, THost, TValue>(string name, TValue defaultValue = default(TValue), bool inherits = false, BindingMode defaultBindingMode = BindingMode.OneWay, Func<TValue, bool>? validate = null, Func<AvaloniaObject, TValue, TValue>? coerce = null) where THost : AvaloniaObject
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		StyledPropertyMetadata<TValue> metadata = new StyledPropertyMetadata<TValue>(defaultValue, defaultBindingMode, coerce);
		AttachedProperty<TValue> attachedProperty = new AttachedProperty<TValue>(name, typeof(TOwner), typeof(THost), metadata, inherits, validate);
		AvaloniaPropertyRegistry instance = AvaloniaPropertyRegistry.Instance;
		instance.Register(typeof(TOwner), attachedProperty);
		instance.RegisterAttached(typeof(THost), attachedProperty);
		return attachedProperty;
	}

	public static AttachedProperty<TValue> RegisterAttached<THost, TValue>(string name, Type ownerType, TValue defaultValue = default(TValue), bool inherits = false, BindingMode defaultBindingMode = BindingMode.OneWay, Func<TValue, bool>? validate = null, Func<AvaloniaObject, TValue, TValue>? coerce = null) where THost : AvaloniaObject
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		StyledPropertyMetadata<TValue> metadata = new StyledPropertyMetadata<TValue>(defaultValue, defaultBindingMode, coerce);
		AttachedProperty<TValue> attachedProperty = new AttachedProperty<TValue>(name, ownerType, typeof(THost), metadata, inherits, validate);
		AvaloniaPropertyRegistry instance = AvaloniaPropertyRegistry.Instance;
		instance.Register(ownerType, attachedProperty);
		instance.RegisterAttached(typeof(THost), attachedProperty);
		return attachedProperty;
	}

	public static DirectProperty<TOwner, TValue> RegisterDirect<TOwner, TValue>(string name, Func<TOwner, TValue> getter, Action<TOwner, TValue>? setter = null, TValue unsetValue = default(TValue), BindingMode defaultBindingMode = BindingMode.OneWay, bool enableDataValidation = false) where TOwner : AvaloniaObject
	{
		if (name == null)
		{
			throw new ArgumentNullException("name");
		}
		if (getter == null)
		{
			throw new ArgumentNullException("getter");
		}
		DirectPropertyMetadata<TValue> metadata = new DirectPropertyMetadata<TValue>(unsetValue, defaultBindingMode, enableDataValidation);
		DirectProperty<TOwner, TValue> directProperty = new DirectProperty<TOwner, TValue>(name, getter, setter, metadata);
		AvaloniaPropertyRegistry.Instance.Register(typeof(TOwner), directProperty);
		return directProperty;
	}

	public IndexerDescriptor Bind()
	{
		return new IndexerDescriptor
		{
			Property = this
		};
	}

	public override bool Equals(object? obj)
	{
		if (obj is AvaloniaProperty other)
		{
			return Equals(other);
		}
		return false;
	}

	public bool Equals(AvaloniaProperty? other)
	{
		return Id == other?.Id;
	}

	public override int GetHashCode()
	{
		return Id;
	}

	public AvaloniaPropertyMetadata GetMetadata<T>() where T : AvaloniaObject
	{
		return GetMetadata(typeof(T));
	}

	public AvaloniaPropertyMetadata GetMetadata(Type type)
	{
		return GetMetadataWithOverrides(type);
	}

	[RequiresUnreferencedCode("Implicit conversion methods are required for type conversion.")]
	public bool IsValidValue(object? value)
	{
		object result;
		return TypeUtilities.TryConvertImplicit(PropertyType, value, out result);
	}

	public override string ToString()
	{
		return Name;
	}

	internal abstract EffectiveValue CreateEffectiveValue(AvaloniaObject o);

	internal abstract void RouteClearValue(AvaloniaObject o);

	internal abstract void RouteCoerceDefaultValue(AvaloniaObject o);

	internal abstract object? RouteGetValue(AvaloniaObject o);

	internal abstract object? RouteGetBaseValue(AvaloniaObject o);

	internal abstract IDisposable? RouteSetValue(AvaloniaObject o, object? value, BindingPriority priority);

	internal abstract void RouteSetCurrentValue(AvaloniaObject o, object? value);

	internal abstract IDisposable RouteBind(AvaloniaObject o, IObservable<object?> source, BindingPriority priority);

	private protected void OverrideMetadata(Type type, AvaloniaPropertyMetadata metadata)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (metadata == null)
		{
			throw new ArgumentNullException("metadata");
		}
		if (_metadata.ContainsKey(type))
		{
			throw new InvalidOperationException($"Metadata is already set for {Name} on {type}.");
		}
		AvaloniaPropertyMetadata metadata2 = GetMetadata(type);
		metadata.Merge(metadata2, this);
		_metadata.Add(type, metadata);
		_metadataCache.Clear();
		_singleMetadata = null;
	}

	private protected abstract IObservable<AvaloniaPropertyChangedEventArgs> GetChanged();

	private AvaloniaPropertyMetadata GetMetadataWithOverrides(Type type)
	{
		if ((object)type == null)
		{
			throw new ArgumentNullException("type");
		}
		if (_metadataCache.TryGetValue(type, out AvaloniaPropertyMetadata value))
		{
			return value;
		}
		KeyValuePair<Type, AvaloniaPropertyMetadata>? singleMetadata = _singleMetadata;
		if (singleMetadata.HasValue)
		{
			KeyValuePair<Type, AvaloniaPropertyMetadata> valueOrDefault = singleMetadata.GetValueOrDefault();
			return _metadataCache[type] = (valueOrDefault.Key.IsAssignableFrom(type) ? valueOrDefault.Value : _defaultMetadata);
		}
		Type type2 = type;
		while (type2 != null)
		{
			if (_metadata.TryGetValue(type2, out value))
			{
				_metadataCache[type] = value;
				return value;
			}
			type2 = type2.BaseType;
		}
		return _metadataCache[type] = _defaultMetadata;
	}

	object? IPropertyInfo.Get(object target)
	{
		return ((AvaloniaObject)target).GetValue(this);
	}

	void IPropertyInfo.Set(object target, object? value)
	{
		((AvaloniaObject)target).SetValue(this, value);
	}
}
public abstract class AvaloniaProperty<TValue> : AvaloniaProperty
{
	private readonly LightweightSubject<AvaloniaPropertyChangedEventArgs<TValue>> _changed;

	public new IObservable<AvaloniaPropertyChangedEventArgs<TValue>> Changed => _changed;

	private protected AvaloniaProperty(string name, Type ownerType, Type hostType, AvaloniaPropertyMetadata metadata, Action<AvaloniaObject, bool>? notifying = null)
		: base(name, typeof(TValue), ownerType, hostType, metadata, notifying)
	{
		_changed = new LightweightSubject<AvaloniaPropertyChangedEventArgs<TValue>>();
	}

	private protected AvaloniaProperty(AvaloniaProperty<TValue> source, Type ownerType, AvaloniaPropertyMetadata? metadata)
		: base(source, ownerType, metadata)
	{
		_changed = source._changed;
	}

	internal void NotifyChanged(AvaloniaPropertyChangedEventArgs<TValue> e)
	{
		_changed.OnNext(e);
	}

	private protected override IObservable<AvaloniaPropertyChangedEventArgs> GetChanged()
	{
		return Changed;
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Implicit conversion methods might be removed by the linker. We don't have a reliable way to prevent it, except converting everything in compile time when possible.")]
	private protected BindingValue<object?> TryConvert(object? value)
	{
		if (value == AvaloniaProperty.UnsetValue)
		{
			return BindingValue<object>.Unset;
		}
		if (value == BindingOperations.DoNothing)
		{
			return BindingValue<object>.DoNothing;
		}
		if (!TypeUtilities.TryConvertImplicit(base.PropertyType, value, out object result))
		{
			return BindingValue<object>.BindingError(new ArgumentException(string.Format("Invalid value for Property '{0}': '{1}' ({2})", base.Name, value, value?.GetType().FullName ?? "(null)")));
		}
		return result;
	}
}
