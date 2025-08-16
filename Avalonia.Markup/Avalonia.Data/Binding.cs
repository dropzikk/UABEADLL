using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers;

namespace Avalonia.Data;

[RequiresUnreferencedCode("BindingExpression and ReflectionBinding heavily use reflection. Consider using CompiledBindings instead.")]
public class Binding : BindingBase
{
	public string? ElementName { get; set; }

	public RelativeSource? RelativeSource { get; set; }

	public object? Source { get; set; }

	public string Path { get; set; } = "";

	public Func<string?, string, Type>? TypeResolver { get; set; }

	public Binding()
	{
	}

	public Binding(string path, BindingMode mode = BindingMode.Default)
		: base(mode)
	{
		Path = path;
	}

	private protected override ExpressionObserver CreateExpressionObserver(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor, bool enableDataValidation)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (anchor == null)
		{
			anchor = base.DefaultAnchor?.Target;
		}
		enableDataValidation = enableDataValidation && base.Priority == BindingPriority.LocalValue;
		INameScope target2 = null;
		base.NameScope?.TryGetTarget(out target2);
		var (expressionNode, sourceMode) = ExpressionObserverBuilder.Parse(Path, enableDataValidation, TypeResolver, target2);
		if (expressionNode == null)
		{
			throw new InvalidOperationException("Could not parse binding expression.");
		}
		if (ElementName != null)
		{
			return CreateElementObserver(GetSource(), ElementName, expressionNode);
		}
		if (Source != null)
		{
			return CreateSourceObserver(Source, expressionNode);
		}
		if (RelativeSource == null)
		{
			if (sourceMode == SourceMode.Data)
			{
				return CreateDataContextObserver(target, expressionNode, targetProperty == StyledElement.DataContextProperty, anchor);
			}
			return CreateSourceObserver(GetSource(), expressionNode);
		}
		if (RelativeSource.Mode == RelativeSourceMode.DataContext)
		{
			return CreateDataContextObserver(target, expressionNode, targetProperty == StyledElement.DataContextProperty, anchor);
		}
		if (RelativeSource.Mode == RelativeSourceMode.Self)
		{
			return CreateSourceObserver(GetSource(), expressionNode);
		}
		if (RelativeSource.Mode == RelativeSourceMode.TemplatedParent)
		{
			return CreateTemplatedParentObserver(GetSource(), expressionNode);
		}
		if (RelativeSource.Mode == RelativeSourceMode.FindAncestor)
		{
			if (RelativeSource.Tree == TreeType.Visual && RelativeSource.AncestorType == null)
			{
				throw new InvalidOperationException("AncestorType must be set for RelativeSourceMode.FindAncestor when searching the visual tree.");
			}
			return CreateFindAncestorObserver(GetSource(), RelativeSource, expressionNode);
		}
		throw new NotSupportedException();
		StyledElement GetSource()
		{
			return (target as StyledElement) ?? (anchor as StyledElement) ?? throw new ArgumentException("Could not find binding source: either target or anchor must be an StyledElement.");
		}
	}
}
