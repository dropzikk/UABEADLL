using System;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Utilities;

namespace Avalonia.Markup.Xaml.MarkupExtensions;

public class ResolveByNameExtension
{
	public string Name { get; }

	public ResolveByNameExtension(string name)
	{
		Name = name;
	}

	public object? ProvideValue(IServiceProvider serviceProvider)
	{
		INameScope service = serviceProvider.GetService<INameScope>();
		if (service == null)
		{
			return null;
		}
		SynchronousCompletionAsyncResult<object?> value = service.FindAsync(Name);
		if (value.IsCompleted)
		{
			return value.GetResult();
		}
		IProvideValueTarget service2 = serviceProvider.GetService<IProvideValueTarget>();
		object obj = service2?.TargetProperty;
		IPropertyInfo property = obj as IPropertyInfo;
		if (property != null)
		{
			object target = service2.TargetObject;
			value.OnCompleted(delegate
			{
				property.Set(target, value.GetResult());
			});
		}
		return AvaloniaProperty.UnsetValue;
	}
}
