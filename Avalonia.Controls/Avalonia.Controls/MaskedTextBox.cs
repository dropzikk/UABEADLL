using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Input.Platform;
using Avalonia.Interactivity;

namespace Avalonia.Controls;

public class MaskedTextBox : TextBox
{
	public static readonly StyledProperty<bool> AsciiOnlyProperty;

	public static readonly StyledProperty<CultureInfo?> CultureProperty;

	public static readonly StyledProperty<bool> HidePromptOnLeaveProperty;

	public static readonly DirectProperty<MaskedTextBox, bool?> MaskCompletedProperty;

	public static readonly DirectProperty<MaskedTextBox, bool?> MaskFullProperty;

	public static readonly StyledProperty<string?> MaskProperty;

	public static readonly StyledProperty<char> PromptCharProperty;

	public static readonly StyledProperty<bool> ResetOnPromptProperty;

	public static readonly StyledProperty<bool> ResetOnSpaceProperty;

	private bool _ignoreTextChanges;

	public bool AsciiOnly
	{
		get
		{
			return GetValue(AsciiOnlyProperty);
		}
		set
		{
			SetValue(AsciiOnlyProperty, value);
		}
	}

	public CultureInfo? Culture
	{
		get
		{
			return GetValue(CultureProperty);
		}
		set
		{
			SetValue(CultureProperty, value);
		}
	}

	public bool HidePromptOnLeave
	{
		get
		{
			return GetValue(HidePromptOnLeaveProperty);
		}
		set
		{
			SetValue(HidePromptOnLeaveProperty, value);
		}
	}

	public string? Mask
	{
		get
		{
			return GetValue(MaskProperty);
		}
		set
		{
			SetValue(MaskProperty, value);
		}
	}

	public bool? MaskCompleted => MaskProvider?.MaskCompleted;

	public bool? MaskFull => MaskProvider?.MaskFull;

	public MaskedTextProvider? MaskProvider { get; private set; }

	public char PromptChar
	{
		get
		{
			return GetValue(PromptCharProperty);
		}
		set
		{
			SetValue(PromptCharProperty, value);
		}
	}

	public bool ResetOnPrompt
	{
		get
		{
			return GetValue(ResetOnPromptProperty);
		}
		set
		{
			SetValue(ResetOnPromptProperty, value);
		}
	}

	public bool ResetOnSpace
	{
		get
		{
			return GetValue(ResetOnSpaceProperty);
		}
		set
		{
			SetValue(ResetOnSpaceProperty, value);
		}
	}

	protected override Type StyleKeyOverride => typeof(TextBox);

	static MaskedTextBox()
	{
		AsciiOnlyProperty = AvaloniaProperty.Register<MaskedTextBox, bool>("AsciiOnly", defaultValue: false);
		CultureProperty = AvaloniaProperty.Register<MaskedTextBox, CultureInfo>("Culture", CultureInfo.CurrentCulture);
		HidePromptOnLeaveProperty = AvaloniaProperty.Register<MaskedTextBox, bool>("HidePromptOnLeave", defaultValue: false);
		MaskCompletedProperty = AvaloniaProperty.RegisterDirect("MaskCompleted", (MaskedTextBox o) => o.MaskCompleted, null, null);
		MaskFullProperty = AvaloniaProperty.RegisterDirect("MaskFull", (MaskedTextBox o) => o.MaskFull, null, null);
		MaskProperty = AvaloniaProperty.Register<MaskedTextBox, string>("Mask", string.Empty);
		PromptCharProperty = AvaloniaProperty.Register<MaskedTextBox, char>("PromptChar", '_', inherits: false, BindingMode.OneWay, null, CoercePromptChar);
		ResetOnPromptProperty = AvaloniaProperty.Register<MaskedTextBox, bool>("ResetOnPrompt", defaultValue: true);
		ResetOnSpaceProperty = AvaloniaProperty.Register<MaskedTextBox, bool>("ResetOnSpace", defaultValue: true);
		TextBox.PasswordCharProperty.OverrideMetadata<MaskedTextBox>(new StyledPropertyMetadata<char>('\0', BindingMode.Default, CoercePasswordChar));
	}

	private static char CoercePasswordChar(AvaloniaObject sender, char baseValue)
	{
		if (!MaskedTextProvider.IsValidPasswordChar(baseValue))
		{
			throw new ArgumentException($"'{baseValue}' is not a valid value for PasswordChar.");
		}
		MaskedTextProvider maskProvider = ((MaskedTextBox)sender).MaskProvider;
		if (maskProvider != null && baseValue == maskProvider.PromptChar)
		{
			throw new InvalidOperationException("PasswordChar and PromptChar values cannot be the same.");
		}
		return baseValue;
	}

	private static char CoercePromptChar(AvaloniaObject sender, char baseValue)
	{
		if (!MaskedTextProvider.IsValidInputChar(baseValue))
		{
			throw new ArgumentException($"'{baseValue}' is not a valid value for PromptChar.");
		}
		if (baseValue == sender.GetValue(TextBox.PasswordCharProperty))
		{
			throw new InvalidOperationException("PasswordChar and PromptChar values cannot be the same.");
		}
		return baseValue;
	}

	public MaskedTextBox()
	{
	}

	public MaskedTextBox(MaskedTextProvider maskedTextProvider)
	{
		if (maskedTextProvider == null)
		{
			throw new ArgumentNullException("maskedTextProvider");
		}
		AsciiOnly = maskedTextProvider.AsciiOnly;
		Culture = maskedTextProvider.Culture;
		Mask = maskedTextProvider.Mask;
		base.PasswordChar = maskedTextProvider.PasswordChar;
		PromptChar = maskedTextProvider.PromptChar;
	}

	protected override void OnGotFocus(GotFocusEventArgs e)
	{
		if (HidePromptOnLeave && MaskProvider != null)
		{
			SetCurrentValue(TextBox.TextProperty, MaskProvider.ToDisplayString());
		}
		base.OnGotFocus(e);
	}

	protected override async void OnKeyDown(KeyEventArgs e)
	{
		if (MaskProvider == null)
		{
			base.OnKeyDown(e);
			return;
		}
		PlatformHotkeyConfiguration platformHotkeyConfiguration = Application.Current.PlatformSettings?.HotkeyConfiguration;
		if (platformHotkeyConfiguration != null && Match(platformHotkeyConfiguration.Paste))
		{
			IClipboard clipboard = TopLevel.GetTopLevel(this)?.Clipboard;
			if (clipboard == null)
			{
				return;
			}
			string text = await clipboard.GetTextAsync();
			if (text == null)
			{
				return;
			}
			string text2 = text;
			foreach (char input in text2)
			{
				int nextCharacterPosition = GetNextCharacterPosition(base.CaretIndex);
				if (MaskProvider.InsertAt(input, nextCharacterPosition))
				{
					SetCurrentValue(TextBox.CaretIndexProperty, nextCharacterPosition + 1);
				}
			}
			SetCurrentValue(TextBox.TextProperty, MaskProvider.ToDisplayString());
			e.Handled = true;
			return;
		}
		if (e.Key != Key.Back)
		{
			base.OnKeyDown(e);
		}
		switch (e.Key)
		{
		case Key.Delete:
			if (base.CaretIndex < base.Text?.Length)
			{
				if (MaskProvider.RemoveAt(base.CaretIndex))
				{
					RefreshText(MaskProvider, base.CaretIndex);
				}
				e.Handled = true;
			}
			break;
		case Key.Space:
			if ((!MaskProvider.ResetOnSpace || string.IsNullOrEmpty(base.SelectedText)) && MaskProvider.InsertAt(" ", base.CaretIndex))
			{
				RefreshText(MaskProvider, base.CaretIndex);
			}
			e.Handled = true;
			break;
		case Key.Back:
			if (base.CaretIndex > 0)
			{
				MaskProvider.RemoveAt(base.CaretIndex - 1);
			}
			RefreshText(MaskProvider, base.CaretIndex - 1);
			e.Handled = true;
			break;
		}
		bool Match(List<KeyGesture> gestures)
		{
			return gestures.Any((KeyGesture g) => g.Matches(e));
		}
	}

	protected override void OnLostFocus(RoutedEventArgs e)
	{
		if (HidePromptOnLeave && MaskProvider != null)
		{
			SetCurrentValue(TextBox.TextProperty, MaskProvider.ToString(!HidePromptOnLeave, includeLiterals: true));
		}
		base.OnLostFocus(e);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (change.Property == TextBox.TextProperty && MaskProvider != null && !_ignoreTextChanges)
		{
			if (string.IsNullOrEmpty(base.Text))
			{
				MaskProvider.Clear();
				RefreshText(MaskProvider, base.CaretIndex);
				base.OnPropertyChanged(change);
				return;
			}
			MaskProvider.Set(base.Text);
			RefreshText(MaskProvider, base.CaretIndex);
		}
		else if (change.Property == MaskProperty)
		{
			UpdateMaskProvider();
			if (!string.IsNullOrEmpty(Mask))
			{
				string mask = Mask;
				for (int i = 0; i < mask.Length; i++)
				{
					if (!MaskedTextProvider.IsValidMaskChar(mask[i]))
					{
						throw new ArgumentException("Specified mask contains characters that are not valid.");
					}
				}
			}
		}
		else if (change.Property == TextBox.PasswordCharProperty)
		{
			if (MaskProvider != null && MaskProvider.PasswordChar != base.PasswordChar)
			{
				UpdateMaskProvider();
			}
		}
		else if (change.Property == PromptCharProperty)
		{
			if (MaskProvider != null && MaskProvider.PromptChar != PromptChar)
			{
				UpdateMaskProvider();
			}
		}
		else if (change.Property == ResetOnPromptProperty)
		{
			if (MaskProvider != null)
			{
				bool newValue = change.GetNewValue<bool>();
				MaskProvider.ResetOnPrompt = newValue;
			}
		}
		else if (change.Property == ResetOnSpaceProperty)
		{
			if (MaskProvider != null)
			{
				bool newValue2 = change.GetNewValue<bool>();
				MaskProvider.ResetOnSpace = newValue2;
			}
		}
		else if ((change.Property == AsciiOnlyProperty && MaskProvider != null && MaskProvider.AsciiOnly != AsciiOnly) || (change.Property == CultureProperty && MaskProvider != null && !MaskProvider.Culture.Equals(Culture)))
		{
			UpdateMaskProvider();
		}
		base.OnPropertyChanged(change);
		void UpdateMaskProvider()
		{
			MaskProvider = new MaskedTextProvider(Mask, Culture, allowPromptAsInput: true, PromptChar, base.PasswordChar, AsciiOnly)
			{
				ResetOnSpace = ResetOnSpace,
				ResetOnPrompt = ResetOnPrompt
			};
			if (base.Text != null)
			{
				MaskProvider.Set(base.Text);
			}
			RefreshText(MaskProvider, 0);
		}
	}

	protected override void OnTextInput(TextInputEventArgs e)
	{
		_ignoreTextChanges = true;
		try
		{
			if (base.IsReadOnly)
			{
				e.Handled = true;
				base.OnTextInput(e);
				return;
			}
			if (MaskProvider == null)
			{
				base.OnTextInput(e);
				return;
			}
			if (((MaskProvider.ResetOnSpace && e.Text == " ") || (MaskProvider.ResetOnPrompt && e.Text == MaskProvider.PromptChar.ToString())) && !string.IsNullOrEmpty(base.SelectedText) && ((base.SelectionStart > base.SelectionEnd) ? MaskProvider.RemoveAt(base.SelectionEnd, base.SelectionStart - 1) : MaskProvider.RemoveAt(base.SelectionStart, base.SelectionEnd - 1)))
			{
				base.SelectedText = string.Empty;
			}
			if (base.CaretIndex < base.Text?.Length)
			{
				SetCurrentValue(TextBox.CaretIndexProperty, GetNextCharacterPosition(base.CaretIndex));
				if (MaskProvider.InsertAt(e.Text, base.CaretIndex))
				{
					base.CaretIndex++;
				}
				int nextCharacterPosition = GetNextCharacterPosition(base.CaretIndex);
				if (nextCharacterPosition != 0 && base.CaretIndex != base.Text.Length)
				{
					SetCurrentValue(TextBox.CaretIndexProperty, nextCharacterPosition);
				}
			}
			RefreshText(MaskProvider, base.CaretIndex);
			e.Handled = true;
			base.OnTextInput(e);
		}
		finally
		{
			_ignoreTextChanges = false;
		}
	}

	private int GetNextCharacterPosition(int startPosition)
	{
		if (MaskProvider != null)
		{
			int result = MaskProvider.FindEditPositionFrom(startPosition, direction: true);
			if (base.CaretIndex != -1)
			{
				return result;
			}
		}
		return startPosition;
	}

	private void RefreshText(MaskedTextProvider? provider, int position)
	{
		if (provider != null)
		{
			SetCurrentValue(TextBox.TextProperty, provider.ToDisplayString());
			SetCurrentValue(TextBox.CaretIndexProperty, position);
		}
	}
}
