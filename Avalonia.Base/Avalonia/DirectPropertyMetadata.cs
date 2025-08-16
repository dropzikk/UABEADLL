using Avalonia.Data;

namespace Avalonia;

public class DirectPropertyMetadata<TValue> : AvaloniaPropertyMetadata, IDirectPropertyMetadata
{
	public TValue UnsetValue { get; private set; }

	object? IDirectPropertyMetadata.UnsetValue => UnsetValue;

	public DirectPropertyMetadata(TValue unsetValue = default(TValue), BindingMode defaultBindingMode = BindingMode.Default, bool? enableDataValidation = null)
		: base(defaultBindingMode, enableDataValidation)
	{
		UnsetValue = unsetValue;
	}

	public override void Merge(AvaloniaPropertyMetadata baseMetadata, AvaloniaProperty property)
	{
		base.Merge(baseMetadata, property);
		if (baseMetadata is DirectPropertyMetadata<TValue> directPropertyMetadata)
		{
			TValue unsetValue = UnsetValue;
			if (unsetValue == null)
			{
				TValue val = (UnsetValue = directPropertyMetadata.UnsetValue);
			}
		}
	}

	public override AvaloniaPropertyMetadata GenerateTypeSafeMetadata()
	{
		return new DirectPropertyMetadata<TValue>(UnsetValue, base.DefaultBindingMode, base.EnableDataValidation);
	}
}
