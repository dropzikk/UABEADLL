using System;
using MicroCom.Runtime;

namespace Avalonia.MicroCom;

public abstract class CallbackBase : IUnknown, IDisposable, IMicroComShadowContainer
{
	private readonly object _lock = new object();

	private bool _referencedFromManaged = true;

	private bool _referencedFromNative;

	private bool _destroyed;

	public bool IsDestroyed => _destroyed;

	public MicroComShadow? Shadow { get; set; }

	protected virtual void Destroyed()
	{
	}

	public void Dispose()
	{
		lock (_lock)
		{
			_referencedFromManaged = false;
			DestroyIfNeeded();
		}
	}

	private void DestroyIfNeeded()
	{
		if (!_destroyed && !_referencedFromManaged && !_referencedFromNative)
		{
			_destroyed = true;
			Shadow?.Dispose();
			Shadow = null;
			Destroyed();
		}
	}

	public void OnReferencedFromNative()
	{
		lock (_lock)
		{
			_referencedFromNative = true;
		}
	}

	public void OnUnreferencedFromNative()
	{
		lock (_lock)
		{
			_referencedFromNative = false;
			DestroyIfNeeded();
		}
	}
}
