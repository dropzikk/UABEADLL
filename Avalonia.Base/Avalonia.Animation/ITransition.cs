using System;
using Avalonia.Metadata;

namespace Avalonia.Animation;

[NotClientImplementable]
[PrivateApi]
public interface ITransition
{
	AvaloniaProperty Property { get; set; }

	internal IDisposable Apply(Animatable control, IClock clock, object? oldValue, object? newValue);
}
