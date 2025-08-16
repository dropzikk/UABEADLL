using System;
using System.ComponentModel;
using Avalonia.Controls.Documents;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Styling;

namespace Avalonia.Controls;

public class DataGridTextColumn : DataGridBoundColumn
{
	private readonly Lazy<ControlTheme> _cellTextBoxTheme;

	private readonly Lazy<ControlTheme> _cellTextBlockTheme;

	public static readonly AttachedProperty<FontFamily> FontFamilyProperty = TextElement.FontFamilyProperty.AddOwner<DataGridTextColumn>();

	public static readonly AttachedProperty<double> FontSizeProperty = TextElement.FontSizeProperty.AddOwner<DataGridTextColumn>();

	public static readonly AttachedProperty<FontStyle> FontStyleProperty = TextElement.FontStyleProperty.AddOwner<DataGridTextColumn>();

	public static readonly AttachedProperty<FontWeight> FontWeightProperty = TextElement.FontWeightProperty.AddOwner<DataGridTextColumn>();

	public static readonly AttachedProperty<FontStretch> FontStretchProperty = TextElement.FontStretchProperty.AddOwner<DataGridTextColumn>();

	public static readonly AttachedProperty<IBrush> ForegroundProperty = TextElement.ForegroundProperty.AddOwner<DataGridTextColumn>();

	public FontFamily FontFamily
	{
		get
		{
			return GetValue(FontFamilyProperty);
		}
		set
		{
			SetValue(FontFamilyProperty, value);
		}
	}

	[DefaultValue(double.NaN)]
	public double FontSize
	{
		get
		{
			return GetValue(FontSizeProperty);
		}
		set
		{
			SetValue(FontSizeProperty, value);
		}
	}

	public FontStyle FontStyle
	{
		get
		{
			return GetValue(FontStyleProperty);
		}
		set
		{
			SetValue(FontStyleProperty, value);
		}
	}

	public FontWeight FontWeight
	{
		get
		{
			return GetValue(FontWeightProperty);
		}
		set
		{
			SetValue(FontWeightProperty, value);
		}
	}

	public FontStretch FontStretch
	{
		get
		{
			return GetValue(FontStretchProperty);
		}
		set
		{
			SetValue(FontStretchProperty, value);
		}
	}

	public IBrush Foreground
	{
		get
		{
			return GetValue(ForegroundProperty);
		}
		set
		{
			SetValue(ForegroundProperty, value);
		}
	}

	public DataGridTextColumn()
	{
		base.BindingTarget = TextBox.TextProperty;
		_cellTextBoxTheme = new Lazy<ControlTheme>(() => (!base.OwningGrid.TryFindResource("DataGridCellTextBoxTheme", out object value)) ? null : ((ControlTheme)value));
		_cellTextBlockTheme = new Lazy<ControlTheme>(() => (!base.OwningGrid.TryFindResource("DataGridCellTextBlockTheme", out object value2)) ? null : ((ControlTheme)value2));
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == FontFamilyProperty || change.Property == FontSizeProperty || change.Property == FontStyleProperty || change.Property == FontWeightProperty || change.Property == ForegroundProperty)
		{
			NotifyPropertyChanged(change.Property.Name);
		}
	}

	protected override void CancelCellEdit(Control editingElement, object uneditedValue)
	{
		if (editingElement is TextBox textBox)
		{
			string text = uneditedValue as string;
			textBox.Text = text ?? string.Empty;
		}
	}

	protected override Control GenerateEditingElementDirect(DataGridCell cell, object dataItem)
	{
		TextBox textBox = new TextBox
		{
			Name = "CellTextBox"
		};
		ControlTheme value = _cellTextBoxTheme.Value;
		if (value != null)
		{
			textBox.Theme = value;
		}
		SyncProperties(textBox);
		return textBox;
	}

	protected override Control GenerateElement(DataGridCell cell, object dataItem)
	{
		TextBlock textBlock = new TextBlock
		{
			Name = "CellTextBlock"
		};
		ControlTheme value = _cellTextBlockTheme.Value;
		if (value != null)
		{
			textBlock.Theme = value;
		}
		SyncProperties(textBlock);
		if (Binding != null)
		{
			textBlock.Bind(TextBlock.TextProperty, Binding);
		}
		return textBlock;
	}

	protected override object PrepareCellForEdit(Control editingElement, RoutedEventArgs editingEventArgs)
	{
		if (editingElement is TextBox textBox)
		{
			string? obj = textBox.Text ?? string.Empty;
			int length = obj.Length;
			if (editingEventArgs is KeyEventArgs { Key: Key.F2 })
			{
				textBox.SelectionStart = length;
				textBox.SelectionEnd = length;
				return obj;
			}
			textBox.SelectionStart = 0;
			textBox.SelectionEnd = length;
			textBox.CaretIndex = length;
			return obj;
		}
		return string.Empty;
	}

	protected internal override void RefreshCellContent(Control element, string propertyName)
	{
		if (element == null)
		{
			throw new ArgumentNullException("element");
		}
		if (element != null)
		{
			switch (propertyName)
			{
			case "FontFamily":
				DataGridHelper.SyncColumnProperty(this, element, FontFamilyProperty);
				break;
			case "FontSize":
				DataGridHelper.SyncColumnProperty(this, element, FontSizeProperty);
				break;
			case "FontStyle":
				DataGridHelper.SyncColumnProperty(this, element, FontStyleProperty);
				break;
			case "FontWeight":
				DataGridHelper.SyncColumnProperty(this, element, FontWeightProperty);
				break;
			case "Foreground":
				DataGridHelper.SyncColumnProperty(this, element, ForegroundProperty);
				break;
			}
			return;
		}
		throw DataGridError.DataGrid.ValueIsNotAnInstanceOf("element", typeof(AvaloniaObject));
	}

	private void SyncProperties(AvaloniaObject content)
	{
		DataGridHelper.SyncColumnProperty(this, content, FontFamilyProperty);
		DataGridHelper.SyncColumnProperty(this, content, FontSizeProperty);
		DataGridHelper.SyncColumnProperty(this, content, FontStyleProperty);
		DataGridHelper.SyncColumnProperty(this, content, FontWeightProperty);
		DataGridHelper.SyncColumnProperty(this, content, ForegroundProperty);
	}
}
