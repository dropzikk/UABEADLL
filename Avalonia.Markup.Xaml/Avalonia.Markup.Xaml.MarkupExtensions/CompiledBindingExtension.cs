using System;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers;
using Avalonia.Markup.Xaml.MarkupExtensions.CompiledBindings;

namespace Avalonia.Markup.Xaml.MarkupExtensions;

public class CompiledBindingExtension : BindingBase
{
	[ConstructorArgument("path")]
	public CompiledBindingPath Path { get; set; }

	public object? Source { get; set; }

	public Type? DataType { get; set; }

	public CompiledBindingExtension()
	{
		Path = new CompiledBindingPath();
	}

	public CompiledBindingExtension(CompiledBindingPath path)
	{
		Path = path;
	}

	public CompiledBindingExtension ProvideValue(IServiceProvider provider)
	{
		return new CompiledBindingExtension
		{
			Path = Path,
			Converter = base.Converter,
			ConverterParameter = base.ConverterParameter,
			TargetNullValue = base.TargetNullValue,
			FallbackValue = base.FallbackValue,
			Mode = base.Mode,
			Priority = base.Priority,
			StringFormat = base.StringFormat,
			Source = Source,
			DefaultAnchor = new WeakReference(provider.GetDefaultAnchor())
		};
	}

	private protected override ExpressionObserver CreateExpressionObserver(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor, bool enableDataValidation)
	{
		if (Source != null)
		{
			return CreateSourceObserver(Source, Path.BuildExpression(enableDataValidation));
		}
		if (Path.RawSource != null)
		{
			return CreateSourceObserver(Path.RawSource, Path.BuildExpression(enableDataValidation));
		}
		if (Path.SourceMode == SourceMode.Data)
		{
			return CreateDataContextObserver(target, Path.BuildExpression(enableDataValidation), targetProperty == StyledElement.DataContextProperty, anchor);
		}
		StyledElement source = (target as StyledElement) ?? (anchor as StyledElement) ?? throw new ArgumentException("Cannot find a valid StyledElement to use as the binding source.");
		return CreateSourceObserver(source, Path.BuildExpression(enableDataValidation));
	}
}
