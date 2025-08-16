using System;
using Avalonia.Platform;

namespace Avalonia.Controls.Platform.Surfaces;

public class FuncFramebufferRenderTarget : IFramebufferRenderTarget, IDisposable
{
	private readonly Func<ILockedFramebuffer> _lockFramebuffer;

	public FuncFramebufferRenderTarget(Func<ILockedFramebuffer> lockFramebuffer)
	{
		_lockFramebuffer = lockFramebuffer;
	}

	public void Dispose()
	{
	}

	public ILockedFramebuffer Lock()
	{
		return _lockFramebuffer();
	}
}
