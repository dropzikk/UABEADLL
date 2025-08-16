using Avalonia.Input.Raw;
using Avalonia.Metadata;

namespace Avalonia.Input;

[PrivateApi]
public interface IPointerDevice : IInputDevice
{
	IPointer? TryGetPointer(RawPointerEventArgs ev);
}
