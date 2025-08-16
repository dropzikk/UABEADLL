using Avalonia.Input.Raw;
using Avalonia.Metadata;

namespace Avalonia.Input;

[NotClientImplementable]
[PrivateApi]
public interface IInputDevice
{
	void ProcessRawEvent(RawInputEventArgs ev);
}
