using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data.Converters;
using Avalonia.Metadata;
using Avalonia.Reactive;

namespace Avalonia.Data;

public class MultiBinding : IBinding
{
	[Content]
	[AssignBinding]
	public IList<IBinding> Bindings { get; set; } = new List<IBinding>();

	public IMultiValueConverter? Converter { get; set; }

	public object? ConverterParameter { get; set; }

	public object FallbackValue { get; set; }

	public object TargetNullValue { get; set; }

	public BindingMode Mode { get; set; } = BindingMode.OneWay;

	public BindingPriority Priority { get; set; }

	public RelativeSource? RelativeSource { get; set; }

	public string? StringFormat { get; set; }

	public MultiBinding()
	{
		FallbackValue = AvaloniaProperty.UnsetValue;
		TargetNullValue = AvaloniaProperty.UnsetValue;
	}

	public InstancedBinding? Initiate(AvaloniaObject target, AvaloniaProperty? targetProperty, object? anchor = null, bool enableDataValidation = false)
	{
		Type targetType = targetProperty?.PropertyType ?? typeof(object);
		IMultiValueConverter converter = Converter;
		if (!string.IsNullOrWhiteSpace(StringFormat) && (targetType == typeof(string) || targetType == typeof(object)))
		{
			converter = new StringFormatMultiValueConverter(StringFormat, converter);
		}
		IObservable<object> observable = from x in (from x in Bindings
				select x.Initiate(target, null) into x
				select x?.Source into x
				where x != null
				select x).CombineLatest()
			select ConvertValue(x, targetType, converter) into x
			where x != BindingOperations.DoNothing
			select x;
		return ((Mode != 0) ? new BindingMode?(Mode) : targetProperty?.GetMetadata(target.GetType()).DefaultBindingMode) switch
		{
			BindingMode.OneTime => InstancedBinding.OneTime(observable, Priority), 
			BindingMode.OneWay => InstancedBinding.OneWay(observable, Priority), 
			_ => throw new NotSupportedException("MultiBinding currently only supports OneTime and OneWay BindingMode."), 
		};
	}

	private object ConvertValue(IList<object?> values, Type targetType, IMultiValueConverter? converter)
	{
		for (int i = 0; i < values.Count; i++)
		{
			if (values[i] is BindingNotification bindingNotification)
			{
				values[i] = bindingNotification.Value;
			}
		}
		CultureInfo currentCulture = CultureInfo.CurrentCulture;
		values = new ReadOnlyCollection<object>(values);
		object obj = ((converter == null) ? values : converter.Convert(values, targetType, ConverterParameter, currentCulture));
		if (obj == null)
		{
			obj = TargetNullValue;
		}
		if (obj == AvaloniaProperty.UnsetValue)
		{
			obj = FallbackValue;
		}
		return obj;
	}
}
