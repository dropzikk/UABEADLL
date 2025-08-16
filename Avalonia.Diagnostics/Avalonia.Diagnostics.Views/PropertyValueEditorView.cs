using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Shapes;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Diagnostics.Controls;
using Avalonia.Diagnostics.ViewModels;
using Avalonia.Input;
using Avalonia.Layout;
using Avalonia.Markup.Xaml.Converters;
using Avalonia.Media;
using Avalonia.Reactive;

namespace Avalonia.Diagnostics.Views;

internal class PropertyValueEditorView : UserControl
{
	private class ValueConverter : IValueConverter
	{
		private bool _firstUpdate = true;

		object? IValueConverter.Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return Convert(value, targetType, parameter, culture);
		}

		object? IValueConverter.ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (_firstUpdate)
			{
				_firstUpdate = false;
				return BindingOperations.DoNothing;
			}
			return ConvertBack(value, (Type)parameter, parameter, culture);
		}

		protected virtual object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value;
		}

		protected virtual object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return value;
		}
	}

	private static class StringConversionHelper
	{
		private const BindingFlags PublicStatic = BindingFlags.Static | BindingFlags.Public;

		private static readonly Type[] StringParameter = new Type[1] { typeof(string) };

		private static readonly Type[] StringFormatProviderParameters = new Type[2]
		{
			typeof(string),
			typeof(IFormatProvider)
		};

		public static bool CanConvertFromString(Type type)
		{
			if (TypeDescriptor.GetConverter(type).CanConvertFrom(typeof(string)))
			{
				return true;
			}
			bool hasFormat;
			return GetParseMethod(type, out hasFormat) != null;
		}

		public static string? ToString(object o)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(o);
			if (!converter.CanConvertTo(typeof(string)) || converter.GetType() == typeof(CollectionConverter))
			{
				return o.ToString();
			}
			return converter.ConvertToInvariantString(o);
		}

		public static object? FromString(string str, Type type)
		{
			TypeConverter converter = TypeDescriptor.GetConverter(type);
			if (!converter.CanConvertFrom(typeof(string)))
			{
				return InvokeParse(str, type);
			}
			return converter.ConvertFrom(null, CultureInfo.InvariantCulture, str);
		}

		private static object? InvokeParse(string s, Type targetType)
		{
			bool hasFormat;
			MethodInfo? parseMethod = GetParseMethod(targetType, out hasFormat);
			if (parseMethod == null)
			{
				throw new InvalidOperationException();
			}
			return parseMethod.Invoke(null, (!hasFormat) ? new object[1] { s } : new object[2]
			{
				s,
				CultureInfo.InvariantCulture
			});
		}

		private static MethodInfo? GetParseMethod(Type type, out bool hasFormat)
		{
			MethodInfo method = type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, StringFormatProviderParameters, null);
			if (method != null)
			{
				hasFormat = true;
				return method;
			}
			hasFormat = false;
			return type.GetMethod("Parse", BindingFlags.Static | BindingFlags.Public, null, StringParameter, null);
		}
	}

	private sealed class ValueToDecimalConverter : ValueConverter
	{
		protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return System.Convert.ToDecimal(value);
		}

		protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			return System.Convert.ChangeType(value, targetType);
		}
	}

	private sealed class TextToValueConverter : ValueConverter
	{
		protected override object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value != null)
			{
				return StringConversionHelper.ToString(value);
			}
			return null;
		}

		protected override object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (!(value is string str))
			{
				return null;
			}
			try
			{
				return StringConversionHelper.FromString(str, targetType);
			}
			catch
			{
				return BindingOperations.DoNothing;
			}
		}
	}

	private static readonly Geometry ImageIcon = Geometry.Parse("M12.25 6C8.79822 6 6 8.79822 6 12.25V35.75C6 37.1059 6.43174 38.3609 7.16525 39.3851L21.5252 25.0251C22.8921 23.6583 25.1081 23.6583 26.475 25.0251L40.8348 39.385C41.5683 38.3608 42 37.1058 42 35.75V12.25C42 8.79822 39.2018 6 35.75 6H12.25ZM34.5 17.5C34.5 19.7091 32.7091 21.5 30.5 21.5C28.2909 21.5 26.5 19.7091 26.5 17.5C26.5 15.2909 28.2909 13.5 30.5 13.5C32.7091 13.5 34.5 15.2909 34.5 17.5ZM39.0024 41.0881L24.7072 26.7929C24.3167 26.4024 23.6835 26.4024 23.293 26.7929L8.99769 41.0882C9.94516 41.6667 11.0587 42 12.25 42H35.75C36.9414 42 38.0549 41.6666 39.0024 41.0881Z");

	private static readonly Geometry GeometryIcon = Geometry.Parse("M23.25 15.5H30.8529C29.8865 8.99258 24.2763 4 17.5 4C10.0442 4 4 10.0442 4 17.5C4 24.2763 8.99258 29.8865 15.5 30.8529V23.25C15.5 18.9698 18.9698 15.5 23.25 15.5ZM23.25 18C20.3505 18 18 20.3505 18 23.25V38.75C18 41.6495 20.3505 44 23.25 44H38.75C41.6495 44 44 41.6495 44 38.75V23.25C44 20.3505 41.6495 18 38.75 18H23.25Z");

	private static readonly ColorToBrushConverter Color2Brush = new ColorToBrushConverter();

	private readonly CompositeDisposable _cleanup = new CompositeDisposable();

	private PropertyViewModel? Property => (PropertyViewModel)base.DataContext;

	protected override void OnDataContextChanged(EventArgs e)
	{
		base.OnDataContextChanged(e);
		base.Content = UpdateControl();
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnDetachedFromVisualTree(e);
		_cleanup.Clear();
	}

	private static bool ImplementsInterface<TInterface>(Type type)
	{
		Type typeFromHandle = typeof(TInterface);
		if (!(type == typeFromHandle))
		{
			return typeFromHandle.IsAssignableFrom(type);
		}
		return true;
	}

	private Control? UpdateControl()
	{
		_cleanup.Clear();
		Type propertyType = Property?.PropertyType;
		if ((object)propertyType == null)
		{
			return null;
		}
		if (propertyType == typeof(bool))
		{
			return CreateControl<CheckBox>(ToggleButton.IsCheckedProperty);
		}
		if (IsValidNumeric(propertyType))
		{
			return CreateControl<NumericUpDown>(NumericUpDown.ValueProperty, new ValueToDecimalConverter(), delegate(NumericUpDown n)
			{
				n.Increment = 1m;
				n.NumberFormat = new NumberFormatInfo
				{
					NumberDecimalDigits = 0
				};
				n.ParsingNumberStyle = NumberStyles.Integer;
			}, NumericUpDown.IsReadOnlyProperty);
		}
		if (propertyType == typeof(Color))
		{
			Ellipse ellipse = new Ellipse
			{
				Width = 12.0,
				Height = 12.0,
				VerticalAlignment = VerticalAlignment.Center
			};
			ellipse.Bind(Shape.FillProperty, new Binding("Value")
			{
				Source = Property,
				Converter = Color2Brush
			}).DisposeWith(_cleanup);
			TextBlock textBlock = new TextBlock
			{
				VerticalAlignment = VerticalAlignment.Center
			};
			textBlock.Bind(TextBlock.TextProperty, new Binding("Value")
			{
				Source = Property
			}).DisposeWith(_cleanup);
			StackPanel sp = new StackPanel
			{
				Orientation = Orientation.Horizontal,
				Spacing = 2.0,
				Children = 
				{
					(Control)ellipse,
					(Control)textBlock
				},
				Background = Brushes.Transparent,
				Cursor = new Cursor(StandardCursorType.Hand),
				IsEnabled = !Property.IsReadonly
			};
			ColorView colorView = new ColorView
			{
				HexInputAlphaPosition = AlphaComponentPosition.Leading
			};
			colorView.Bind(ColorView.ColorProperty, new Binding("Value", BindingMode.TwoWay)
			{
				Source = Property,
				Converter = Color2Brush
			}).DisposeWith(_cleanup);
			FlyoutBase.SetAttachedFlyout(sp, new Flyout
			{
				Content = colorView
			});
			sp.PointerPressed += delegate
			{
				FlyoutBase.ShowAttachedFlyout(sp);
			};
			return sp;
		}
		if (ImplementsInterface<IBrush>(propertyType))
		{
			return CreateControl<BrushEditor>(BrushEditor.BrushProperty);
		}
		bool flag = ImplementsInterface<IImage>(propertyType);
		bool flag2 = propertyType == typeof(Geometry);
		if (flag || flag2)
		{
			IObservable<object> observable = Property.GetObservable((PropertyViewModel x) => x.Value);
			TextBlock textBlock2 = new TextBlock
			{
				VerticalAlignment = VerticalAlignment.Center
			};
			textBlock2.Bind(TextBlock.TextProperty, observable.Select(delegate(object value)
			{
				if (value is IImage image)
				{
					return $"{image.Size.Width} x {image.Size.Height}";
				}
				return (value is Geometry geometry) ? $"{geometry.Bounds.Width} x {geometry.Bounds.Height}" : "(null)";
			})).DisposeWith(_cleanup);
			StackPanel stackPanel = new StackPanel
			{
				Background = Brushes.Transparent,
				Orientation = Orientation.Horizontal,
				Spacing = 2.0,
				Children = 
				{
					(Control)new Path
					{
						Data = (flag ? ImageIcon : GeometryIcon),
						Fill = Brushes.Gray,
						Width = 12.0,
						Height = 12.0,
						Stretch = Stretch.Uniform,
						VerticalAlignment = VerticalAlignment.Center
					},
					(Control)textBlock2
				}
			};
			if (flag)
			{
				Image image2 = new Image
				{
					Stretch = Stretch.Uniform,
					Width = 300.0,
					Height = 300.0
				};
				image2.Bind(Image.SourceProperty, observable).DisposeWith(_cleanup);
				ToolTip.SetTip(stackPanel, image2);
			}
			else
			{
				Path path = new Path
				{
					Stretch = Stretch.Uniform,
					Fill = Brushes.White,
					VerticalAlignment = VerticalAlignment.Center,
					HorizontalAlignment = HorizontalAlignment.Center
				};
				path.Bind(Path.DataProperty, observable).DisposeWith(_cleanup);
				ToolTip.SetTip(stackPanel, new Border
				{
					Child = path,
					Width = 300.0,
					Height = 300.0
				});
			}
			return stackPanel;
		}
		if (propertyType.IsEnum)
		{
			return CreateControl<ComboBox>(SelectingItemsControl.SelectedItemProperty, null, delegate(ComboBox c)
			{
				c.ItemsSource = Enum.GetValues(propertyType);
			});
		}
		CommitTextBox tb = CreateControl<CommitTextBox>(CommitTextBox.CommittedTextProperty, new TextToValueConverter(), delegate(CommitTextBox t)
		{
			t.Watermark = "(null)";
		}, TextBox.IsReadOnlyProperty);
		tb.IsReadOnly |= propertyType == typeof(object) || !StringConversionHelper.CanConvertFromString(propertyType);
		if (!tb.IsReadOnly)
		{
			tb.GetObservable(TextBox.TextProperty).Subscribe(delegate(string t)
			{
				try
				{
					if (t != null)
					{
						StringConversionHelper.FromString(t, propertyType);
					}
					DataValidationErrors.ClearErrors(tb);
				}
				catch (Exception ex)
				{
					DataValidationErrors.SetError(tb, ex.GetBaseException());
				}
			}).DisposeWith(_cleanup);
		}
		return tb;
		TControl CreateControl<TControl>(AvaloniaProperty valueProperty, IValueConverter? converter = null, Action<TControl>? init = null, AvaloniaProperty? readonlyProperty = null) where TControl : notnull, Control, new()
		{
			TControl val = new TControl();
			BindingMode mode = (Property.IsReadonly ? BindingMode.OneWay : BindingMode.TwoWay);
			init?.Invoke(val);
			val.Bind(valueProperty, new Binding("Value", mode)
			{
				Source = Property,
				Converter = (converter ?? new ValueConverter()),
				ConverterParameter = propertyType
			}).DisposeWith(_cleanup);
			if (readonlyProperty != null)
			{
				val[readonlyProperty] = Property.IsReadonly;
			}
			else
			{
				val.IsEnabled = !Property.IsReadonly;
			}
			return val;
		}
		static bool IsValidNumeric(Type? type)
		{
			if (type == null || type.IsEnum)
			{
				return false;
			}
			TypeCode typeCode = Type.GetTypeCode(type);
			if (typeCode == TypeCode.Object)
			{
				if (!type.IsGenericType || !(type.GetGenericTypeDefinition() == typeof(Nullable<>)))
				{
					return false;
				}
				typeCode = Type.GetTypeCode(Nullable.GetUnderlyingType(type));
			}
			if ((uint)(typeCode - 5) <= 8u)
			{
				return true;
			}
			return false;
		}
	}
}
