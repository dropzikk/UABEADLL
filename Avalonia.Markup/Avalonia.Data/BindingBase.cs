using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Controls;
using Avalonia.Data.Converters;
using Avalonia.Data.Core;
using Avalonia.LogicalTree;
using Avalonia.Reactive;
using Avalonia.VisualTree;

namespace Avalonia.Data;

public abstract class BindingBase : IBinding
{
	private class UpdateSignal : SingleSubscriberObservableBase<ValueTuple>
	{
		private readonly AvaloniaObject _target;

		private readonly AvaloniaProperty _property;

		public UpdateSignal(AvaloniaObject target, AvaloniaProperty property)
		{
			_target = target;
			_property = property;
		}

		protected override void Subscribed()
		{
			_target.PropertyChanged += PropertyChanged;
		}

		protected override void Unsubscribed()
		{
			_target.PropertyChanged -= PropertyChanged;
		}

		private void PropertyChanged(object? sender, AvaloniaPropertyChangedEventArgs e)
		{
			if (e.Property == _property)
			{
				PublishNext(default(ValueTuple));
			}
		}
	}

	public IValueConverter? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public object? FallbackValue { get; set; }

	public object? TargetNullValue { get; set; }

	public BindingMode Mode { get; set; }

	public BindingPriority Priority { get; set; }

	public string? StringFormat { get; set; }

	public WeakReference? DefaultAnchor { get; set; }

	public WeakReference<INameScope?>? NameScope { get; set; }

	public BindingBase()
	{
		FallbackValue = AvaloniaProperty.UnsetValue;
		TargetNullValue = AvaloniaProperty.UnsetValue;
	}

	public BindingBase(BindingMode mode = BindingMode.Default)
		: this()
	{
		Mode = mode;
	}

	private protected abstract ExpressionObserver CreateExpressionObserver(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor, bool enableDataValidation);

	[UnconditionalSuppressMessage("Trimming", "IL2026", Justification = "Conversion methods might be removed by the linker. We don't have a reliable way to prevent it, except converting everything in compile time when possible.")]
	public InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		anchor = anchor ?? DefaultAnchor?.Target;
		enableDataValidation = enableDataValidation && Priority == BindingPriority.LocalValue;
		ExpressionObserver inner = CreateExpressionObserver(target, targetProperty, anchor, enableDataValidation);
		object obj = FallbackValue;
		if (targetProperty == StyledElement.DataContextProperty && obj == AvaloniaProperty.UnsetValue)
		{
			obj = null;
		}
		IValueConverter valueConverter = Converter;
		Type type = targetProperty?.PropertyType ?? typeof(object);
		if (!string.IsNullOrWhiteSpace(StringFormat) && (type == typeof(string) || type == typeof(object)))
		{
			valueConverter = new StringFormatValueConverter(StringFormat, valueConverter);
		}
		return new InstancedBinding(new BindingExpression(inner, type, obj, TargetNullValue, valueConverter ?? DefaultValueConverter.Instance, ConverterParameter, Priority), Mode, Priority);
	}

	private protected ExpressionObserver CreateDataContextObserver(AvaloniaObject target, ExpressionNode node, bool targetIsDataContext, object? anchor)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (!(target is IDataContextProvider))
		{
			if (!(anchor is IDataContextProvider) || !(anchor is AvaloniaObject avaloniaObject))
			{
				throw new InvalidOperationException("Cannot find a DataContext to bind to.");
			}
			target = avaloniaObject;
		}
		if (!targetIsDataContext)
		{
			return new ExpressionObserver(() => target.GetValue(StyledElement.DataContextProperty), node, new UpdateSignal(target, StyledElement.DataContextProperty), null);
		}
		return new ExpressionObserver(GetParentDataContext(target), node, null);
	}

	private protected ExpressionObserver CreateElementObserver(StyledElement target, string elementName, ExpressionNode node)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		if (NameScope == null || !NameScope.TryGetTarget(out INameScope target2))
		{
			throw new InvalidOperationException("Name scope is null or was already collected");
		}
		return new ExpressionObserver(NameScopeLocator.Track(target2, elementName), node, null);
	}

	private protected ExpressionObserver CreateFindAncestorObserver(StyledElement target, RelativeSource relativeSource, ExpressionNode node)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		return new ExpressionObserver(relativeSource.Tree switch
		{
			TreeType.Logical => ControlLocator.Track(target, relativeSource.AncestorLevel - 1, relativeSource.AncestorType), 
			TreeType.Visual => VisualLocator.Track((Visual)target, relativeSource.AncestorLevel - 1, relativeSource.AncestorType), 
			_ => throw new InvalidOperationException("Invalid tree to traverse."), 
		}, node, null);
	}

	private protected ExpressionObserver CreateSourceObserver(object source, ExpressionNode node)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		return new ExpressionObserver(source, node);
	}

	private protected ExpressionObserver CreateTemplatedParentObserver(AvaloniaObject target, ExpressionNode node)
	{
		if (target == null)
		{
			throw new ArgumentNullException("target");
		}
		return new ExpressionObserver(() => target.GetValue(StyledElement.TemplatedParentProperty), node, new UpdateSignal(target, StyledElement.TemplatedParentProperty), null);
	}

	private IObservable<object?> GetParentDataContext(AvaloniaObject target)
	{
		return (from x in target.GetObservable(Visual.VisualParentProperty)
			select x?.GetObservable(StyledElement.DataContextProperty) ?? Observable.Return<object>(null)).Switch();
	}
}
