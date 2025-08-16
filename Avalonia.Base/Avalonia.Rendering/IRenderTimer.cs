using System;
using Avalonia.Metadata;

namespace Avalonia.Rendering;

[PrivateApi]
public interface IRenderTimer
{
	bool RunsInBackground { get; }

	event Action<TimeSpan> Tick;
}
