using Avalonia.Metadata;

namespace Avalonia.Input.TextInput;

[NotClientImplementable]
public interface ITextInputMethodRoot : IInputRoot, IInputElement
{
	ITextInputMethodImpl? InputMethod { get; }
}
