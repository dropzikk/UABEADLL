using Avalonia.Metadata;

namespace Avalonia.Input;

[NotClientImplementable]
public interface IFocusManager
{
	IInputElement? GetFocusedElement();

	[Unstable("This API might be removed in 11.x minor updates. Please consider focusing another element instead of removing focus at all for better UX.")]
	void ClearFocus();
}
