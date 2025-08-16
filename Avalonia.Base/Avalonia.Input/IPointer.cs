using Avalonia.Metadata;

namespace Avalonia.Input;

[NotClientImplementable]
public interface IPointer
{
	int Id { get; }

	IInputElement? Captured { get; }

	PointerType Type { get; }

	bool IsPrimary { get; }

	void Capture(IInputElement? control);
}
