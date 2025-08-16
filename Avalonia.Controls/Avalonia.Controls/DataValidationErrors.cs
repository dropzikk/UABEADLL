using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Templates;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia.Controls;

[PseudoClasses(new string[] { ":error" })]
public class DataValidationErrors : ContentControl
{
	public static readonly AttachedProperty<IEnumerable<object>?> ErrorsProperty;

	public static readonly AttachedProperty<bool> HasErrorsProperty;

	public static readonly StyledProperty<IDataTemplate> ErrorTemplateProperty;

	private Control? _owner;

	public static readonly DirectProperty<DataValidationErrors, Control?> OwnerProperty;

	public Control? Owner
	{
		get
		{
			return _owner;
		}
		set
		{
			SetAndRaise(OwnerProperty, ref _owner, value);
		}
	}

	public IDataTemplate ErrorTemplate
	{
		get
		{
			return GetValue(ErrorTemplateProperty);
		}
		set
		{
			SetValue(ErrorTemplateProperty, value);
		}
	}

	static DataValidationErrors()
	{
		ErrorsProperty = AvaloniaProperty.RegisterAttached<DataValidationErrors, Control, IEnumerable<object>>("Errors");
		HasErrorsProperty = AvaloniaProperty.RegisterAttached<DataValidationErrors, Control, bool>("HasErrors", defaultValue: false);
		ErrorTemplateProperty = AvaloniaProperty.Register<DataValidationErrors, IDataTemplate>("ErrorTemplate");
		OwnerProperty = AvaloniaProperty.RegisterDirect("Owner", (DataValidationErrors o) => o.Owner, delegate(DataValidationErrors o, Control? v)
		{
			o.Owner = v;
		});
		ErrorsProperty.Changed.Subscribe(ErrorsChanged);
		HasErrorsProperty.Changed.Subscribe(HasErrorsChanged);
		StyledElement.TemplatedParentProperty.Changed.AddClassHandler(delegate(DataValidationErrors x, AvaloniaPropertyChangedEventArgs e)
		{
			x.OnTemplatedParentChange(e);
		});
	}

	private void OnTemplatedParentChange(AvaloniaPropertyChangedEventArgs e)
	{
		if (Owner == null)
		{
			Owner = e.NewValue as Control;
		}
	}

	private static void ErrorsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		Control obj = (Control)e.Sender;
		IEnumerable<object> enumerable = (IEnumerable<object>)e.NewValue;
		bool value = false;
		if (enumerable != null && enumerable.Any())
		{
			value = true;
		}
		obj.SetValue(HasErrorsProperty, value);
	}

	private static void HasErrorsChanged(AvaloniaPropertyChangedEventArgs e)
	{
		PseudolassesExtensions.Set(((Control)e.Sender).Classes, ":error", (bool)e.NewValue);
	}

	public static IEnumerable<object>? GetErrors(Control control)
	{
		return control.GetValue(ErrorsProperty);
	}

	public static void SetErrors(Control control, IEnumerable<object>? errors)
	{
		control.SetValue(ErrorsProperty, errors);
	}

	public static void SetError(Control control, Exception? error)
	{
		SetErrors(control, UnpackException(error));
	}

	public static void ClearErrors(Control control)
	{
		SetErrors(control, null);
	}

	public static bool GetHasErrors(Control control)
	{
		return control.GetValue(HasErrorsProperty);
	}

	private static IEnumerable<object>? UnpackException(Exception? exception)
	{
		if (exception != null)
		{
			List<object> list = ((!(exception is AggregateException ex)) ? ((IEnumerable<object>)new object[1] { GetExceptionData(exception) }) : ((IEnumerable<object>)ex.InnerExceptions.Select(GetExceptionData).ToArray())).Where((object x) => !(x is BindingChainException)).ToList();
			if (list.Count > 0)
			{
				return list;
			}
		}
		return null;
	}

	private static object GetExceptionData(Exception exception)
	{
		if (exception is DataValidationException ex)
		{
			object errorData = ex.ErrorData;
			if (errorData != null)
			{
				return errorData;
			}
		}
		return exception;
	}
}
