using System;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Markup.Xaml.MarkupExtensions;

public class DynamicResourceExtension : IBinding
{
	private object? _anchor;

	private BindingPriority _priority;

	private ThemeVariant? _currentThemeVariant;

	public object? ResourceKey { get; set; }

	public DynamicResourceExtension()
	{
	}

	public DynamicResourceExtension(object resourceKey)
	{
		ResourceKey = resourceKey;
	}

	public IBinding ProvideValue(IServiceProvider serviceProvider)
	{
		if (serviceProvider.IsInControlTemplate())
		{
			_priority = BindingPriority.Template;
		}
		if (!(serviceProvider.GetService<IProvideValueTarget>()?.TargetObject is StyledElement))
		{
			_anchor = serviceProvider.GetFirstParent<StyledElement>() ?? ((object)serviceProvider.GetFirstParent<IResourceProvider>()) ?? ((object)serviceProvider.GetFirstParent<IResourceHost>());
		}
		_currentThemeVariant = StaticResourceExtension.GetDictionaryVariant(serviceProvider);
		return this;
	}

	InstancedBinding? IBinding.Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor, bool enableDataValidation)
	{
		if (ResourceKey == null)
		{
			return null;
		}
		IResourceHost resourceHost = (target as IResourceHost) ?? (_anchor as IResourceHost);
		if (resourceHost != null)
		{
			return InstancedBinding.OneWay(resourceHost.GetResourceObservable(ResourceKey, GetConverter(targetProperty)), _priority);
		}
		if (_anchor is IResourceProvider resourceProvider)
		{
			return InstancedBinding.OneWay(resourceProvider.GetResourceObservable(ResourceKey, _currentThemeVariant, GetConverter(targetProperty)), _priority);
		}
		return null;
	}

	private static Func<object?, object?>? GetConverter(AvaloniaProperty? targetProperty)
	{
		if (targetProperty?.PropertyType == typeof(IBrush))
		{
			return (object? x) => ColorToBrushConverter.Convert(x, typeof(IBrush));
		}
		return null;
	}
}
