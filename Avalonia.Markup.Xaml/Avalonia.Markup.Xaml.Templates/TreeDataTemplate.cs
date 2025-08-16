using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Data.Core;
using Avalonia.Markup.Parsers;
using Avalonia.Markup.Xaml.MarkupExtensions;
using Avalonia.Metadata;

namespace Avalonia.Markup.Xaml.Templates;

public class TreeDataTemplate : ITreeDataTemplate, IDataTemplate, ITemplate<object?, Control?>, ITypedDataTemplate
{
	[DataType]
	public Type? DataType { get; set; }

	[Content]
	[TemplateContent]
	public object? Content { get; set; }

	[AssignBinding]
	public BindingBase? ItemsSource { get; set; }

	public bool Match(object? data)
	{
		if (DataType == null)
		{
			return true;
		}
		return DataType.IsInstanceOfType(data);
	}

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "If ItemsSource is a CompiledBinding, then path members will be preserver")]
	public InstancedBinding? ItemsSelector(object item)
	{
		if (ItemsSource != null)
		{
			BindingBase itemsSource = ItemsSource;
			ExpressionObserver observable;
			if (!(itemsSource is Binding binding))
			{
				if (!(itemsSource is CompiledBindingExtension compiledBindingExtension))
				{
					throw new InvalidOperationException("TreeDataTemplate currently only supports Binding and CompiledBindingExtension!");
				}
				observable = new ExpressionObserver(item, compiledBindingExtension.Path.BuildExpression(enableValidation: false));
			}
			else
			{
				observable = ExpressionObserverBuilder.Build(item, binding.Path);
			}
			return InstancedBinding.OneWay(observable, BindingPriority.Style);
		}
		return null;
	}

	public Control? Build(object? data)
	{
		Control control = TemplateContent.Load(Content)?.Result;
		if (control != null)
		{
			control.DataContext = data;
		}
		return control;
	}
}
