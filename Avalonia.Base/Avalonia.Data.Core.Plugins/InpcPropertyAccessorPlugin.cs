using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using Avalonia.Utilities;

namespace Avalonia.Data.Core.Plugins;

internal class InpcPropertyAccessorPlugin : IPropertyAccessorPlugin
{
	private class Accessor : PropertyAccessorBase, IWeakEventSubscriber<PropertyChangedEventArgs>
	{
		private readonly WeakReference<object?> _reference;

		private readonly PropertyInfo _property;

		private bool _eventRaised;

		public override Type? PropertyType => _property.PropertyType;

		public override object? Value
		{
			get
			{
				object referenceTarget = GetReferenceTarget();
				if (referenceTarget == null)
				{
					return null;
				}
				return _property.GetValue(referenceTarget);
			}
		}

		public Accessor(WeakReference<object?> reference, PropertyInfo property)
		{
			if (reference == null)
			{
				throw new ArgumentNullException("reference");
			}
			if ((object)property == null)
			{
				throw new ArgumentNullException("property");
			}
			_reference = reference;
			_property = property;
		}

		public override bool SetValue(object? value, BindingPriority priority)
		{
			if (_property.CanWrite)
			{
				_eventRaised = false;
				_property.SetValue(GetReferenceTarget(), value);
				if (!_eventRaised)
				{
					SendCurrentValue();
				}
				return true;
			}
			return false;
		}

		void IWeakEventSubscriber<PropertyChangedEventArgs>.OnEvent(object? notifyPropertyChanged, WeakEvent ev, PropertyChangedEventArgs e)
		{
			if (e.PropertyName == _property.Name || string.IsNullOrEmpty(e.PropertyName))
			{
				_eventRaised = true;
				SendCurrentValue();
			}
		}

		protected override void SubscribeCore()
		{
			SubscribeToChanges();
			SendCurrentValue();
		}

		protected override void UnsubscribeCore()
		{
			if (GetReferenceTarget() is INotifyPropertyChanged target)
			{
				WeakEvents.ThreadSafePropertyChanged.Unsubscribe(target, this);
			}
		}

		private object? GetReferenceTarget()
		{
			_reference.TryGetTarget(out object target);
			return target;
		}

		private void SendCurrentValue()
		{
			try
			{
				object value = Value;
				PublishValue(value);
			}
			catch
			{
			}
		}

		private void SubscribeToChanges()
		{
			if (GetReferenceTarget() is INotifyPropertyChanged target)
			{
				WeakEvents.ThreadSafePropertyChanged.Subscribe(target, this);
			}
		}
	}

	private readonly Dictionary<(Type, string), PropertyInfo?> _propertyLookup = new Dictionary<(Type, string), PropertyInfo>();

	private const BindingFlags PropertyBindingFlags = BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic;

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public bool Match(object obj, string propertyName)
	{
		return GetFirstPropertyWithName(obj, propertyName) != null;
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	public IPropertyAccessor? Start(WeakReference<object?> reference, string propertyName)
	{
		if (reference == null)
		{
			throw new ArgumentNullException("reference");
		}
		if (propertyName == null)
		{
			throw new ArgumentNullException("propertyName");
		}
		if (!reference.TryGetTarget(out object target) || target == null)
		{
			return null;
		}
		PropertyInfo firstPropertyWithName = GetFirstPropertyWithName(target, propertyName);
		if (firstPropertyWithName != null)
		{
			return new Accessor(reference, firstPropertyWithName);
		}
		return new PropertyError(new BindingNotification(new MissingMemberException($"Could not find CLR property '{propertyName}' on '{target}'"), BindingErrorType.Error));
	}

	[RequiresUnreferencedCode("PropertyAccessors might require unreferenced code.")]
	private PropertyInfo? GetFirstPropertyWithName(object instance, string propertyName)
	{
		if (instance is IReflectableType reflectableType && !(instance is Type))
		{
			return reflectableType.GetTypeInfo().GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		}
		Type type = instance.GetType();
		(Type, string) key = (type, propertyName);
		if (!_propertyLookup.TryGetValue(key, out PropertyInfo value))
		{
			return TryFindAndCacheProperty(type, propertyName);
		}
		return value;
	}

	private PropertyInfo? TryFindAndCacheProperty([DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicProperties | DynamicallyAccessedMemberTypes.NonPublicProperties)] Type type, string propertyName)
	{
		PropertyInfo propertyInfo = null;
		PropertyInfo[] properties = type.GetProperties(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		foreach (PropertyInfo propertyInfo2 in properties)
		{
			if (propertyInfo2.Name == propertyName)
			{
				propertyInfo = propertyInfo2;
				break;
			}
		}
		_propertyLookup.Add((type, propertyName), propertyInfo);
		return propertyInfo;
	}
}
