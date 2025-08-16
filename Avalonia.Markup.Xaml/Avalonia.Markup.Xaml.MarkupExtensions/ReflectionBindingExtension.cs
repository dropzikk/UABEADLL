using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace Avalonia.Markup.Xaml.MarkupExtensions;

[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
public class ReflectionBindingExtension
{
	public IValueConverter? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public string? ElementName { get; set; }

	public object? FallbackValue { get; set; } = AvaloniaProperty.UnsetValue;

	public BindingMode Mode { get; set; }

	[ConstructorArgument("path")]
	public string Path { get; set; } = "";

	public BindingPriority Priority { get; set; }

	public object? Source { get; set; }

	public string? StringFormat { get; set; }

	public RelativeSource? RelativeSource { get; set; }

	public object? TargetNullValue { get; set; } = AvaloniaProperty.UnsetValue;

	public ReflectionBindingExtension()
	{
	}

	public ReflectionBindingExtension(string path)
	{
		Path = path;
	}

	public Binding ProvideValue(IServiceProvider serviceProvider)
	{
		return new Binding
		{
			TypeResolver = serviceProvider.ResolveType,
			Converter = Converter,
			ConverterParameter = ConverterParameter,
			ElementName = ElementName,
			FallbackValue = FallbackValue,
			Mode = Mode,
			Path = Path,
			Priority = Priority,
			Source = Source,
			StringFormat = StringFormat,
			RelativeSource = RelativeSource,
			DefaultAnchor = new WeakReference(serviceProvider.GetDefaultAnchor()),
			TargetNullValue = TargetNullValue,
			NameScope = new WeakReference<INameScope>(serviceProvider.GetService<INameScope>())
		};
	}
}
