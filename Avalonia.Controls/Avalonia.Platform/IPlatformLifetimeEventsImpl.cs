using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Metadata;

namespace Avalonia.Platform;

[Unstable]
public interface IPlatformLifetimeEventsImpl
{
	event EventHandler<ShutdownRequestedEventArgs>? ShutdownRequested;
}
