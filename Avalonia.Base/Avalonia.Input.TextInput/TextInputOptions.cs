namespace Avalonia.Input.TextInput;

public class TextInputOptions
{
	public static readonly TextInputOptions Default = new TextInputOptions();

	public static readonly AttachedProperty<TextInputContentType> ContentTypeProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, TextInputContentType>("ContentType", TextInputContentType.Normal, inherits: true);

	public static readonly AttachedProperty<TextInputReturnKeyType> ReturnKeyTypeProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, TextInputReturnKeyType>("ReturnKeyType", TextInputReturnKeyType.Default, inherits: true);

	public static readonly AttachedProperty<bool> MultilineProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, bool>("Multiline", defaultValue: false, inherits: true);

	public static readonly AttachedProperty<bool> LowercaseProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, bool>("Lowercase", defaultValue: false, inherits: true);

	public static readonly AttachedProperty<bool> UppercaseProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, bool>("Uppercase", defaultValue: false, inherits: true);

	public static readonly AttachedProperty<bool> AutoCapitalizationProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, bool>("AutoCapitalization", defaultValue: false, inherits: true);

	public static readonly AttachedProperty<bool> IsSensitiveProperty = AvaloniaProperty.RegisterAttached<TextInputOptions, StyledElement, bool>("IsSensitive", defaultValue: false, inherits: true);

	public TextInputContentType ContentType { get; set; }

	public TextInputReturnKeyType ReturnKeyType { get; set; }

	public bool Multiline { get; set; }

	public bool Lowercase { get; set; }

	public bool Uppercase { get; set; }

	public bool AutoCapitalization { get; set; }

	public bool IsSensitive { get; set; }

	public static TextInputOptions FromStyledElement(StyledElement avaloniaObject)
	{
		return new TextInputOptions
		{
			ContentType = GetContentType(avaloniaObject),
			ReturnKeyType = GetReturnKeyType(avaloniaObject),
			Multiline = GetMultiline(avaloniaObject),
			AutoCapitalization = GetAutoCapitalization(avaloniaObject),
			IsSensitive = GetIsSensitive(avaloniaObject),
			Lowercase = GetLowercase(avaloniaObject),
			Uppercase = GetUppercase(avaloniaObject)
		};
	}

	public static void SetContentType(StyledElement avaloniaObject, TextInputContentType value)
	{
		avaloniaObject.SetValue(ContentTypeProperty, value);
	}

	public static TextInputContentType GetContentType(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(ContentTypeProperty);
	}

	public static void SetReturnKeyType(StyledElement avaloniaObject, TextInputReturnKeyType value)
	{
		avaloniaObject.SetValue(ReturnKeyTypeProperty, value);
	}

	public static TextInputReturnKeyType GetReturnKeyType(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(ReturnKeyTypeProperty);
	}

	public static void SetMultiline(StyledElement avaloniaObject, bool value)
	{
		avaloniaObject.SetValue(MultilineProperty, value);
	}

	public static bool GetMultiline(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(MultilineProperty);
	}

	public static void SetLowercase(StyledElement avaloniaObject, bool value)
	{
		avaloniaObject.SetValue(LowercaseProperty, value);
	}

	public static bool GetLowercase(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(LowercaseProperty);
	}

	public static void SetUppercase(StyledElement avaloniaObject, bool value)
	{
		avaloniaObject.SetValue(UppercaseProperty, value);
	}

	public static bool GetUppercase(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(UppercaseProperty);
	}

	public static void SetAutoCapitalization(StyledElement avaloniaObject, bool value)
	{
		avaloniaObject.SetValue(AutoCapitalizationProperty, value);
	}

	public static bool GetAutoCapitalization(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(AutoCapitalizationProperty);
	}

	public static void SetIsSensitive(StyledElement avaloniaObject, bool value)
	{
		avaloniaObject.SetValue(IsSensitiveProperty, value);
	}

	public static bool GetIsSensitive(StyledElement avaloniaObject)
	{
		return avaloniaObject.GetValue(IsSensitiveProperty);
	}
}
