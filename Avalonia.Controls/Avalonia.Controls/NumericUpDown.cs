using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Data.Converters;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Layout;
using Avalonia.Reactive;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Controls;

[TemplatePart("PART_Spinner", typeof(Spinner))]
[TemplatePart("PART_TextBox", typeof(TextBox))]
public class NumericUpDown : TemplatedControl
{
	public static readonly StyledProperty<bool> AllowSpinProperty;

	public static readonly StyledProperty<Location> ButtonSpinnerLocationProperty;

	public static readonly StyledProperty<bool> ShowButtonSpinnerProperty;

	public static readonly StyledProperty<bool> ClipValueToMinMaxProperty;

	public static readonly StyledProperty<NumberFormatInfo?> NumberFormatProperty;

	public static readonly StyledProperty<string> FormatStringProperty;

	public static readonly StyledProperty<decimal> IncrementProperty;

	public static readonly StyledProperty<bool> IsReadOnlyProperty;

	public static readonly StyledProperty<decimal> MaximumProperty;

	public static readonly StyledProperty<decimal> MinimumProperty;

	public static readonly StyledProperty<NumberStyles> ParsingNumberStyleProperty;

	public static readonly StyledProperty<string?> TextProperty;

	public static readonly StyledProperty<IValueConverter?> TextConverterProperty;

	public static readonly StyledProperty<decimal?> ValueProperty;

	public static readonly StyledProperty<string?> WatermarkProperty;

	public static readonly StyledProperty<HorizontalAlignment> HorizontalContentAlignmentProperty;

	public static readonly StyledProperty<VerticalAlignment> VerticalContentAlignmentProperty;

	private IDisposable? _textBoxTextChangedSubscription;

	private bool _internalValueSet;

	private bool _isSyncingTextAndValueProperties;

	private bool _isTextChangedFromUI;

	public static readonly RoutedEvent<NumericUpDownValueChangedEventArgs> ValueChangedEvent;

	private Spinner? Spinner { get; set; }

	private TextBox? TextBox { get; set; }

	public bool AllowSpin
	{
		get
		{
			return GetValue(AllowSpinProperty);
		}
		set
		{
			SetValue(AllowSpinProperty, value);
		}
	}

	public Location ButtonSpinnerLocation
	{
		get
		{
			return GetValue(ButtonSpinnerLocationProperty);
		}
		set
		{
			SetValue(ButtonSpinnerLocationProperty, value);
		}
	}

	public bool ShowButtonSpinner
	{
		get
		{
			return GetValue(ShowButtonSpinnerProperty);
		}
		set
		{
			SetValue(ShowButtonSpinnerProperty, value);
		}
	}

	public bool ClipValueToMinMax
	{
		get
		{
			return GetValue(ClipValueToMinMaxProperty);
		}
		set
		{
			SetValue(ClipValueToMinMaxProperty, value);
		}
	}

	public NumberFormatInfo? NumberFormat
	{
		get
		{
			return GetValue(NumberFormatProperty);
		}
		set
		{
			SetValue(NumberFormatProperty, value);
		}
	}

	public string FormatString
	{
		get
		{
			return GetValue(FormatStringProperty);
		}
		set
		{
			SetValue(FormatStringProperty, value);
		}
	}

	public decimal Increment
	{
		get
		{
			return GetValue(IncrementProperty);
		}
		set
		{
			SetValue(IncrementProperty, value);
		}
	}

	public bool IsReadOnly
	{
		get
		{
			return GetValue(IsReadOnlyProperty);
		}
		set
		{
			SetValue(IsReadOnlyProperty, value);
		}
	}

	public decimal Maximum
	{
		get
		{
			return GetValue(MaximumProperty);
		}
		set
		{
			SetValue(MaximumProperty, value);
		}
	}

	public decimal Minimum
	{
		get
		{
			return GetValue(MinimumProperty);
		}
		set
		{
			SetValue(MinimumProperty, value);
		}
	}

	public NumberStyles ParsingNumberStyle
	{
		get
		{
			return GetValue(ParsingNumberStyleProperty);
		}
		set
		{
			SetValue(ParsingNumberStyleProperty, value);
		}
	}

	public string? Text
	{
		get
		{
			return GetValue(TextProperty);
		}
		set
		{
			SetValue(TextProperty, value);
		}
	}

	public IValueConverter? TextConverter
	{
		get
		{
			return GetValue(TextConverterProperty);
		}
		set
		{
			SetValue(TextConverterProperty, value);
		}
	}

	public decimal? Value
	{
		get
		{
			return GetValue(ValueProperty);
		}
		set
		{
			SetValue(ValueProperty, value);
		}
	}

	public string? Watermark
	{
		get
		{
			return GetValue(WatermarkProperty);
		}
		set
		{
			SetValue(WatermarkProperty, value);
		}
	}

	public HorizontalAlignment HorizontalContentAlignment
	{
		get
		{
			return GetValue(HorizontalContentAlignmentProperty);
		}
		set
		{
			SetValue(HorizontalContentAlignmentProperty, value);
		}
	}

	public VerticalAlignment VerticalContentAlignment
	{
		get
		{
			return GetValue(VerticalContentAlignmentProperty);
		}
		set
		{
			SetValue(VerticalContentAlignmentProperty, value);
		}
	}

	public event EventHandler<SpinEventArgs>? Spinned;

	public event EventHandler<NumericUpDownValueChangedEventArgs>? ValueChanged
	{
		add
		{
			AddHandler(ValueChangedEvent, value);
		}
		remove
		{
			RemoveHandler(ValueChangedEvent, value);
		}
	}

	public NumericUpDown()
	{
		base.Initialized += delegate
		{
			if (!_internalValueSet && base.IsInitialized)
			{
				SyncTextAndValueProperties(updateValueFromText: false, null, forceTextUpdate: true);
			}
			SetValidSpinDirection();
		};
	}

	static NumericUpDown()
	{
		AllowSpinProperty = ButtonSpinner.AllowSpinProperty.AddOwner<NumericUpDown>();
		ButtonSpinnerLocationProperty = ButtonSpinner.ButtonSpinnerLocationProperty.AddOwner<NumericUpDown>();
		ShowButtonSpinnerProperty = ButtonSpinner.ShowButtonSpinnerProperty.AddOwner<NumericUpDown>();
		ClipValueToMinMaxProperty = AvaloniaProperty.Register<NumericUpDown, bool>("ClipValueToMinMax", defaultValue: false);
		NumberFormatProperty = AvaloniaProperty.Register<NumericUpDown, NumberFormatInfo>("NumberFormat", NumberFormatInfo.CurrentInfo);
		FormatStringProperty = AvaloniaProperty.Register<NumericUpDown, string>("FormatString", string.Empty);
		IncrementProperty = AvaloniaProperty.Register<NumericUpDown, decimal>("Increment", 1.0m, inherits: false, BindingMode.OneWay, null, OnCoerceIncrement);
		IsReadOnlyProperty = AvaloniaProperty.Register<NumericUpDown, bool>("IsReadOnly", defaultValue: false);
		MaximumProperty = AvaloniaProperty.Register<NumericUpDown, decimal>("Maximum", decimal.MaxValue, inherits: false, BindingMode.OneWay, null, OnCoerceMaximum);
		MinimumProperty = AvaloniaProperty.Register<NumericUpDown, decimal>("Minimum", decimal.MinValue, inherits: false, BindingMode.OneWay, null, OnCoerceMinimum);
		ParsingNumberStyleProperty = AvaloniaProperty.Register<NumericUpDown, NumberStyles>("ParsingNumberStyle", NumberStyles.Any);
		TextProperty = AvaloniaProperty.Register<NumericUpDown, string>("Text", null, inherits: false, BindingMode.TwoWay, null, null, enableDataValidation: true);
		TextConverterProperty = AvaloniaProperty.Register<NumericUpDown, IValueConverter>("TextConverter");
		ValueProperty = AvaloniaProperty.Register<NumericUpDown, decimal?>("Value", null, inherits: false, BindingMode.TwoWay, null, (AvaloniaObject s, decimal? v) => ((NumericUpDown)s).OnCoerceValue(v), enableDataValidation: true);
		WatermarkProperty = AvaloniaProperty.Register<NumericUpDown, string>("Watermark");
		HorizontalContentAlignmentProperty = ContentControl.HorizontalContentAlignmentProperty.AddOwner<NumericUpDown>();
		VerticalContentAlignmentProperty = ContentControl.VerticalContentAlignmentProperty.AddOwner<NumericUpDown>();
		ValueChangedEvent = RoutedEvent.Register<NumericUpDown, NumericUpDownValueChangedEventArgs>("ValueChanged", RoutingStrategies.Bubble);
		NumberFormatProperty.Changed.Subscribe(OnNumberFormatChanged);
		FormatStringProperty.Changed.Subscribe(FormatStringChanged);
		IncrementProperty.Changed.Subscribe(IncrementChanged);
		IsReadOnlyProperty.Changed.Subscribe(OnIsReadOnlyChanged);
		MaximumProperty.Changed.Subscribe(OnMaximumChanged);
		MinimumProperty.Changed.Subscribe(OnMinimumChanged);
		TextProperty.Changed.Subscribe(OnTextChanged);
		TextConverterProperty.Changed.Subscribe(OnTextConverterChanged);
		ValueProperty.Changed.Subscribe(OnValueChanged);
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		CommitInput(forceTextUpdate: true);
		base.OnLostFocus(e);
	}

	protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
	{
		if (TextBox != null)
		{
			TextBox.PointerPressed -= TextBoxOnPointerPressed;
			_textBoxTextChangedSubscription?.Dispose();
		}
		TextBox = e.NameScope.Find<TextBox>("PART_TextBox");
		if (TextBox != null)
		{
			TextBox.Text = Text;
			TextBox.PointerPressed += TextBoxOnPointerPressed;
			_textBoxTextChangedSubscription = TextBox.GetObservable(Avalonia.Controls.TextBox.TextProperty).Subscribe(delegate
			{
				TextBoxOnTextChanged();
			});
		}
		if (Spinner != null)
		{
			Spinner.Spin -= OnSpinnerSpin;
		}
		Spinner = e.NameScope.Find<Spinner>("PART_Spinner");
		if (Spinner != null)
		{
			Spinner.Spin += OnSpinnerSpin;
		}
		SetValidSpinDirection();
	}

	protected override void OnKeyDown(KeyEventArgs e)
	{
		if (e.Key == Key.Return)
		{
			bool flag = CommitInput();
			e.Handled = !flag;
		}
	}

	protected override void UpdateDataValidation(AvaloniaProperty property, BindingValueType state, Exception? error)
	{
		if (property == TextProperty || property == ValueProperty)
		{
			DataValidationErrors.SetError(this, error);
		}
	}

	protected virtual void OnNumberFormatChanged(NumberFormatInfo? oldValue, NumberFormatInfo? newValue)
	{
		if (base.IsInitialized)
		{
			SyncTextAndValueProperties(updateValueFromText: false, null);
		}
	}

	protected virtual void OnFormatStringChanged(string? oldValue, string? newValue)
	{
		if (base.IsInitialized)
		{
			SyncTextAndValueProperties(updateValueFromText: false, null);
		}
	}

	protected virtual void OnIncrementChanged(decimal oldValue, decimal newValue)
	{
		if (base.IsInitialized)
		{
			SetValidSpinDirection();
		}
	}

	protected virtual void OnIsReadOnlyChanged(bool oldValue, bool newValue)
	{
		SetValidSpinDirection();
	}

	protected virtual void OnMaximumChanged(decimal oldValue, decimal newValue)
	{
		if (base.IsInitialized)
		{
			SetValidSpinDirection();
		}
		if (ClipValueToMinMax && Value.HasValue)
		{
			SetCurrentValue(ValueProperty, MathUtilities.Clamp(Value.Value, Minimum, Maximum));
		}
	}

	protected virtual void OnMinimumChanged(decimal oldValue, decimal newValue)
	{
		if (base.IsInitialized)
		{
			SetValidSpinDirection();
		}
		if (ClipValueToMinMax && Value.HasValue)
		{
			SetCurrentValue(ValueProperty, MathUtilities.Clamp(Value.Value, Minimum, Maximum));
		}
	}

	protected virtual void OnTextChanged(string? oldValue, string? newValue)
	{
		if (base.IsInitialized)
		{
			SyncTextAndValueProperties(updateValueFromText: true, Text);
		}
	}

	protected virtual void OnTextConverterChanged(IValueConverter? oldValue, IValueConverter? newValue)
	{
		if (base.IsInitialized)
		{
			SyncTextAndValueProperties(updateValueFromText: false, null);
		}
	}

	protected virtual void OnValueChanged(decimal? oldValue, decimal? newValue)
	{
		if (!_internalValueSet && base.IsInitialized)
		{
			SyncTextAndValueProperties(updateValueFromText: false, null, forceTextUpdate: true);
		}
		SetValidSpinDirection();
		RaiseValueChangedEvent(oldValue, newValue);
	}

	protected virtual decimal OnCoerceIncrement(decimal baseValue)
	{
		return baseValue;
	}

	protected virtual decimal OnCoerceMaximum(decimal baseValue)
	{
		return Math.Max(baseValue, Minimum);
	}

	protected virtual decimal OnCoerceMinimum(decimal baseValue)
	{
		return Math.Min(baseValue, Maximum);
	}

	protected virtual decimal? OnCoerceValue(decimal? baseValue)
	{
		return baseValue;
	}

	protected virtual void OnSpin(SpinEventArgs e)
	{
		if (e == null)
		{
			throw new ArgumentNullException("e");
		}
		this.Spinned?.Invoke(this, e);
		if (e.Direction == SpinDirection.Increase)
		{
			DoIncrement();
		}
		else
		{
			DoDecrement();
		}
	}

	protected virtual void RaiseValueChangedEvent(decimal? oldValue, decimal? newValue)
	{
		NumericUpDownValueChangedEventArgs e = new NumericUpDownValueChangedEventArgs(ValueChangedEvent, oldValue, newValue);
		RaiseEvent(e);
	}

	private decimal? ConvertTextToValue(string? text)
	{
		decimal? result = null;
		if (string.IsNullOrEmpty(text))
		{
			return result;
		}
		string text2 = ConvertValueToText();
		if (object.Equals(text2, text))
		{
			return Value;
		}
		result = ConvertTextToValueCore(text2, text);
		if (ClipValueToMinMax && result.HasValue)
		{
			return MathUtilities.Clamp(result.Value, Minimum, Maximum);
		}
		ValidateMinMax(result);
		return result;
	}

	private string? ConvertValueToText()
	{
		if (TextConverter != null)
		{
			return TextConverter.ConvertBack(Value, typeof(string), null, CultureInfo.CurrentCulture)?.ToString();
		}
		if (FormatString.Contains("{0"))
		{
			return string.Format(NumberFormat, FormatString, Value);
		}
		return Value?.ToString(FormatString, NumberFormat);
	}

	private void OnIncrement()
	{
		SetCurrentValue(value: MathUtilities.Clamp((!Value.HasValue) ? (IsSet(MinimumProperty) ? Minimum : 0m) : (Value.Value + Increment), Minimum, Maximum), property: ValueProperty);
	}

	private void OnDecrement()
	{
		SetCurrentValue(value: MathUtilities.Clamp((!Value.HasValue) ? (IsSet(MaximumProperty) ? Maximum : 0m) : (Value.Value - Increment), Minimum, Maximum), property: ValueProperty);
	}

	private void SetValidSpinDirection()
	{
		ValidSpinDirections validSpinDirections = ValidSpinDirections.None;
		if (Increment != 0m && !IsReadOnly)
		{
			if (!Value.HasValue)
			{
				validSpinDirections = ValidSpinDirections.Increase | ValidSpinDirections.Decrease;
			}
			decimal? value = Value;
			decimal maximum = Maximum;
			if ((value.GetValueOrDefault() < maximum) & value.HasValue)
			{
				validSpinDirections |= ValidSpinDirections.Increase;
			}
			value = Value;
			maximum = Minimum;
			if ((value.GetValueOrDefault() > maximum) & value.HasValue)
			{
				validSpinDirections |= ValidSpinDirections.Decrease;
			}
		}
		if (Spinner != null)
		{
			Spinner.ValidSpinDirection = validSpinDirections;
		}
	}

	private static void OnNumberFormatChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			NumberFormatInfo oldValue = (NumberFormatInfo)e.OldValue;
			NumberFormatInfo newValue = (NumberFormatInfo)e.NewValue;
			numericUpDown.OnNumberFormatChanged(oldValue, newValue);
		}
	}

	private static void IncrementChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			decimal oldValue = (decimal)e.OldValue;
			decimal newValue = (decimal)e.NewValue;
			numericUpDown.OnIncrementChanged(oldValue, newValue);
		}
	}

	private static void FormatStringChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			string oldValue = (string)e.OldValue;
			string newValue = (string)e.NewValue;
			numericUpDown.OnFormatStringChanged(oldValue, newValue);
		}
	}

	private static void OnIsReadOnlyChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			bool oldValue = (bool)e.OldValue;
			bool newValue = (bool)e.NewValue;
			numericUpDown.OnIsReadOnlyChanged(oldValue, newValue);
		}
	}

	private static void OnMaximumChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			decimal oldValue = (decimal)e.OldValue;
			decimal newValue = (decimal)e.NewValue;
			numericUpDown.OnMaximumChanged(oldValue, newValue);
		}
	}

	private static void OnMinimumChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			decimal oldValue = (decimal)e.OldValue;
			decimal newValue = (decimal)e.NewValue;
			numericUpDown.OnMinimumChanged(oldValue, newValue);
		}
	}

	private static void OnTextChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			string oldValue = (string)e.OldValue;
			string newValue = (string)e.NewValue;
			numericUpDown.OnTextChanged(oldValue, newValue);
		}
	}

	private static void OnTextConverterChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			IValueConverter oldValue = (IValueConverter)e.OldValue;
			IValueConverter newValue = (IValueConverter)e.NewValue;
			numericUpDown.OnTextConverterChanged(oldValue, newValue);
		}
	}

	private static void OnValueChanged(AvaloniaPropertyChangedEventArgs e)
	{
		if (e.Sender is NumericUpDown numericUpDown)
		{
			decimal? oldValue = (decimal?)e.OldValue;
			decimal? newValue = (decimal?)e.NewValue;
			numericUpDown.OnValueChanged(oldValue, newValue);
		}
	}

	private void SetValueInternal(decimal? value)
	{
		_internalValueSet = true;
		try
		{
			SetCurrentValue(ValueProperty, value);
		}
		finally
		{
			_internalValueSet = false;
		}
	}

	private static decimal OnCoerceMaximum(AvaloniaObject instance, decimal value)
	{
		if (instance is NumericUpDown numericUpDown)
		{
			return numericUpDown.OnCoerceMaximum(value);
		}
		return value;
	}

	private static decimal OnCoerceMinimum(AvaloniaObject instance, decimal value)
	{
		if (instance is NumericUpDown numericUpDown)
		{
			return numericUpDown.OnCoerceMinimum(value);
		}
		return value;
	}

	private static decimal OnCoerceIncrement(AvaloniaObject instance, decimal value)
	{
		if (instance is NumericUpDown numericUpDown)
		{
			return numericUpDown.OnCoerceIncrement(value);
		}
		return value;
	}

	private void TextBoxOnTextChanged()
	{
		try
		{
			_isTextChangedFromUI = true;
			if (TextBox != null)
			{
				SetCurrentValue(TextProperty, TextBox.Text);
			}
		}
		finally
		{
			_isTextChangedFromUI = false;
		}
	}

	private void OnSpinnerSpin(object? sender, SpinEventArgs e)
	{
		if (AllowSpin && !IsReadOnly && (!e.UsingMouseWheel | (TextBox != null && TextBox.IsFocused)))
		{
			e.Handled = true;
			OnSpin(e);
		}
	}

	private void DoDecrement()
	{
		if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Decrease) == ValidSpinDirections.Decrease)
		{
			OnDecrement();
		}
	}

	private void DoIncrement()
	{
		if (Spinner == null || (Spinner.ValidSpinDirection & ValidSpinDirections.Increase) == ValidSpinDirections.Increase)
		{
			OnIncrement();
		}
	}

	private void TextBoxOnPointerPressed(object? sender, PointerPressedEventArgs e)
	{
		if (e.Pointer.Captured != Spinner)
		{
			Dispatcher.UIThread.InvokeAsync(delegate
			{
				e.Pointer.Capture(Spinner);
			}, DispatcherPriority.Input);
		}
	}

	private bool CommitInput(bool forceTextUpdate = false)
	{
		return SyncTextAndValueProperties(updateValueFromText: true, Text, forceTextUpdate);
	}

	private bool SyncTextAndValueProperties(bool updateValueFromText, string? text)
	{
		return SyncTextAndValueProperties(updateValueFromText, text, forceTextUpdate: false);
	}

	private bool SyncTextAndValueProperties(bool updateValueFromText, string? text, bool forceTextUpdate)
	{
		if (_isSyncingTextAndValueProperties)
		{
			return true;
		}
		_isSyncingTextAndValueProperties = true;
		bool flag = true;
		try
		{
			if (updateValueFromText)
			{
				try
				{
					decimal? num = ConvertTextToValue(text);
					if (!object.Equals(num, Value))
					{
						SetValueInternal(num);
					}
				}
				catch
				{
					flag = false;
				}
			}
			if (!_isTextChangedFromUI)
			{
				if (forceTextUpdate)
				{
					string text2 = ConvertValueToText();
					if (!object.Equals(Text, text2))
					{
						SetCurrentValue(TextProperty, text2);
					}
				}
				if (TextBox != null)
				{
					TextBox.Text = Text;
				}
			}
			if (_isTextChangedFromUI && !flag)
			{
				if (Spinner != null)
				{
					Spinner.ValidSpinDirection = ValidSpinDirections.None;
				}
			}
			else
			{
				SetValidSpinDirection();
			}
		}
		finally
		{
			_isSyncingTextAndValueProperties = false;
		}
		return flag;
	}

	private decimal? ConvertTextToValueCore(string? currentValueText, string? text)
	{
		if (string.IsNullOrEmpty(text))
		{
			return null;
		}
		if (TextConverter != null)
		{
			return (decimal?)TextConverter.Convert(text, typeof(decimal?), null, CultureInfo.CurrentCulture);
		}
		decimal value;
		if (IsPercent(FormatString))
		{
			value = ParsePercent(text, NumberFormat);
		}
		else
		{
			if (!decimal.TryParse(text, ParsingNumberStyle, NumberFormat, out var result))
			{
				bool flag = true;
				if (!string.IsNullOrEmpty(currentValueText) && !decimal.TryParse(currentValueText, ParsingNumberStyle, NumberFormat, out var _))
				{
					IEnumerable<char> first = currentValueText.Where((char c) => !char.IsDigit(c));
					IEnumerable<char> enumerable = text.Where((char c) => !char.IsDigit(c));
					if (!first.Except(enumerable).Any())
					{
						foreach (char item in enumerable)
						{
							text = text.Replace(item.ToString(), string.Empty);
						}
						if (decimal.TryParse(text, ParsingNumberStyle, NumberFormat, out result))
						{
							flag = false;
						}
					}
				}
				if (flag)
				{
					throw new InvalidDataException("Input string was not in a correct format.");
				}
			}
			value = result;
		}
		return value;
	}

	private void ValidateMinMax(decimal? value)
	{
		if (value.HasValue)
		{
			decimal? num = value;
			decimal minimum = Minimum;
			if ((num.GetValueOrDefault() < minimum) & num.HasValue)
			{
				throw new ArgumentOutOfRangeException("value", $"Value must be greater than Minimum value of {Minimum}");
			}
			num = value;
			minimum = Maximum;
			if ((num.GetValueOrDefault() > minimum) & num.HasValue)
			{
				throw new ArgumentOutOfRangeException("value", $"Value must be less than Maximum value of {Maximum}");
			}
		}
	}

	private static decimal ParsePercent(string text, IFormatProvider? cultureInfo)
	{
		NumberFormatInfo instance = NumberFormatInfo.GetInstance(cultureInfo);
		text = text.Replace(instance.PercentSymbol, null);
		return decimal.Parse(text, NumberStyles.Any, instance) / 100m;
	}

	private bool IsPercent(string stringToTest)
	{
		int num = stringToTest.IndexOf("P", StringComparison.Ordinal);
		if (num >= 0)
		{
			return !stringToTest.Substring(0, num).Contains('\'') || !stringToTest.Substring(num, FormatString.Length - num).Contains('\'');
		}
		return false;
	}
}
