using System;
using Avalonia.Input.Raw;
using Avalonia.Metadata;

namespace Avalonia.Input;

[NotClientImplementable]
[PrivateApi]
public interface IInputManager
{
	IObservable<RawInputEventArgs> PreProcess { get; }

	IObservable<RawInputEventArgs> Process { get; }

	IObservable<RawInputEventArgs> PostProcess { get; }

	void ProcessInput(RawInputEventArgs e);
}
