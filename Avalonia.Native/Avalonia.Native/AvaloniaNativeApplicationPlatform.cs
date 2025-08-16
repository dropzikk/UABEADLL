using System;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using MicroCom.Runtime;

namespace Avalonia.Native;

internal class AvaloniaNativeApplicationPlatform : NativeCallbackBase, IAvnApplicationEvents, IUnknown, IDisposable, IPlatformLifetimeEventsImpl
{
	public event EventHandler<ShutdownRequestedEventArgs> ShutdownRequested;

	void IAvnApplicationEvents.FilesOpened(IAvnStringArray urls)
	{
		((IApplicationPlatformEvents)Application.Current).RaiseUrlsOpened(urls.ToStringArray());
	}

	public int TryShutdown()
	{
		if (this.ShutdownRequested == null)
		{
			return 1;
		}
		ShutdownRequestedEventArgs shutdownRequestedEventArgs = new ShutdownRequestedEventArgs();
		this.ShutdownRequested(this, shutdownRequestedEventArgs);
		return (!shutdownRequestedEventArgs.Cancel).AsComBool();
	}
}
