using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Markup.Data;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Markup.Xaml.XamlIl.Runtime;
using Avalonia.Styling;

namespace Avalonia.Markup.Xaml.MarkupExtensions;

public class StaticResourceExtension
{
	public object? ResourceKey { get; set; }

	public StaticResourceExtension()
	{
	}

	public StaticResourceExtension(object resourceKey)
	{
		ResourceKey = resourceKey;
	}

	public object? ProvideValue(IServiceProvider serviceProvider)
	{
		object resourceKey = ResourceKey;
		if (resourceKey == null)
		{
			throw new ArgumentException("StaticResourceExtension.ResourceKey must be set.");
		}
		IAvaloniaXamlIlParentStackProvider service = serviceProvider.GetService<IAvaloniaXamlIlParentStackProvider>();
		IProvideValueTarget? service2 = serviceProvider.GetService<IProvideValueTarget>();
		object obj = service2?.TargetObject;
		object obj2 = service2?.TargetProperty;
		ThemeVariant theme = (obj as IThemeVariantHost)?.ActualThemeVariant ?? GetDictionaryVariant(serviceProvider);
		Type type = ((obj2 is AvaloniaProperty avaloniaProperty) ? avaloniaProperty.PropertyType : ((!(obj2 is PropertyInfo propertyInfo)) ? null : propertyInfo.PropertyType));
		Type type2 = type;
		if (obj is Setter setter)
		{
			AvaloniaProperty property = setter.Property;
			if ((object)property != null)
			{
				type2 = property.PropertyType;
			}
		}
		if (service != null)
		{
			foreach (object parent in service.Parents)
			{
				if (parent is IResourceNode resourceNode && resourceNode.TryGetResource(resourceKey, theme, out object value))
				{
					return ColorToBrushConverter.Convert(value, type2);
				}
			}
		}
		if (obj is Control target && obj2 is PropertyInfo property2)
		{
			Type localTargetType = type2;
			DelayedBinding.Add(target, property2, (StyledElement x) => GetValue(x, localTargetType));
			return AvaloniaProperty.UnsetValue;
		}
		throw new KeyNotFoundException($"Static resource '{resourceKey}' not found.");
	}

	private object? GetValue(StyledElement control, Type? targetType)
	{
		return ColorToBrushConverter.Convert(control.FindResource(ResourceKey), targetType);
	}

	internal static ThemeVariant? GetDictionaryVariant(IServiceProvider serviceProvider)
	{
		IEnumerable<object> enumerable = serviceProvider.GetService<IAvaloniaXamlIlParentStackProvider>()?.Parents;
		if (enumerable == null)
		{
			return null;
		}
		foreach (object item in enumerable)
		{
			if (item is IThemeVariantProvider themeVariantProvider)
			{
				ThemeVariant key = themeVariantProvider.Key;
				if ((object)key != null)
				{
					return key;
				}
			}
		}
		return null;
	}
}
