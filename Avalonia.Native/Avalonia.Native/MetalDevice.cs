using System;
using Avalonia.Metal;
using Avalonia.Native.Interop;
using Avalonia.Platform;
using Avalonia.Utilities;

namespace Avalonia.Native;

internal class MetalDevice : IMetalDevice, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	private DisposableLock _syncRoot = new DisposableLock();

	public IAvnMetalDevice Native { get; private set; }

	public bool IsLost => false;

	public IntPtr Device => Native.Device;

	public IntPtr CommandQueue => Native.Queue;

	public MetalDevice(IAvnMetalDevice native)
	{
		Native = native;
	}

	public void Dispose()
	{
		Native?.Dispose();
		Native = null;
	}

	public object TryGetFeature(Type featureType)
	{
		return null;
	}

	public IDisposable EnsureCurrent()
	{
		return _syncRoot.Lock();
	}
}
