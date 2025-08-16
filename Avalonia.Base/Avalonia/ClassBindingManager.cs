using System;
using System.Collections.Generic;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia;

internal static class ClassBindingManager
{
	private static readonly Dictionary<string, AvaloniaProperty> s_RegisteredProperties = new Dictionary<string, AvaloniaProperty>();

	public static IDisposable Bind(StyledElement target, string className, IBinding source, object anchor)
	{
		if (!s_RegisteredProperties.TryGetValue(className, out AvaloniaProperty value))
		{
			value = (s_RegisteredProperties[className] = RegisterClassProxyProperty(className));
		}
		return target.Bind(value, source, anchor);
	}

	private static AvaloniaProperty RegisterClassProxyProperty(string className)
	{
		StyledProperty<bool> styledProperty = AvaloniaProperty.Register<StyledElement, bool>("__AvaloniaReserved::Classes::" + className, defaultValue: false);
		styledProperty.Changed.Subscribe(delegate(AvaloniaPropertyChangedEventArgs<bool> args)
		{
			((StyledElement)args.Sender).Classes.Set(className, args.NewValue.GetValueOrDefault());
		});
		return styledProperty;
	}
}
