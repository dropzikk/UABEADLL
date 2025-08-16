using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.OpenGL.Egl;

public class EglContext : IGlContext, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	private class RestoreContext : IDisposable
	{
		private readonly EglInterface _egl;

		private readonly object _l;

		private readonly IntPtr _display;

		private readonly IntPtr _context;

		private readonly IntPtr _read;

		private readonly IntPtr _draw;

		public RestoreContext(EglInterface egl, IntPtr defDisplay, object l)
		{
			_egl = egl;
			_l = l;
			_display = _egl.GetCurrentDisplay();
			if (_display == IntPtr.Zero)
			{
				_display = defDisplay;
			}
			_context = _egl.GetCurrentContext();
			_read = _egl.GetCurrentSurface(12378);
			_draw = _egl.GetCurrentSurface(12377);
		}

		public void Dispose()
		{
			_egl.MakeCurrent(_display, _draw, _read, _context);
			Monitor.Exit(_l);
		}
	}

	private readonly EglDisplay _disp;

	private readonly EglInterface _egl;

	private readonly EglContext? _sharedWith;

	private bool _isLost;

	private IntPtr _context;

	private readonly Action? _disposeCallback;

	private readonly Dictionary<Type, object> _features;

	private readonly object _lock;

	public IntPtr Context
	{
		get
		{
			if (!(_context == IntPtr.Zero))
			{
				return _context;
			}
			throw new ObjectDisposedException("EglContext");
		}
	}

	public EglSurface? OffscreenSurface { get; }

	public GlVersion Version { get; }

	public GlInterface GlInterface { get; }

	public int SampleCount { get; }

	public int StencilSize { get; }

	public EglDisplay Display => _disp;

	public bool IsLost
	{
		get
		{
			if (!_isLost && !_disp.IsLost)
			{
				return Context == IntPtr.Zero;
			}
			return true;
		}
	}

	public bool CanCreateSharedContext => _disp.SupportsSharing;

	public bool IsCurrent
	{
		get
		{
			if (_egl.GetCurrentDisplay() == _disp.Handle)
			{
				return _egl.GetCurrentContext() == Context;
			}
			return false;
		}
	}

	internal EglContext(EglDisplay display, EglInterface egl, EglContext? sharedWith, IntPtr ctx, EglSurface? offscreenSurface, GlVersion version, int sampleCount, int stencilSize, Action? disposeCallback, Dictionary<Type, Func<EglContext, object>> features)
	{
		_disp = display;
		_egl = egl;
		_sharedWith = sharedWith;
		_context = ctx;
		_disposeCallback = disposeCallback;
		OffscreenSurface = offscreenSurface;
		Version = version;
		SampleCount = sampleCount;
		StencilSize = stencilSize;
		_lock = display.ContextSharedSyncRoot ?? new object();
		using (MakeCurrent())
		{
			GlInterface = Avalonia.OpenGL.GlInterface.FromNativeUtf8GetProcAddress(version, _egl.GetProcAddress);
			_features = features.ToDictionary<KeyValuePair<Type, Func<EglContext, object>>, Type, object>((KeyValuePair<Type, Func<EglContext, object>> x) => x.Key, (KeyValuePair<Type, Func<EglContext, object>> x) => x.Value(this));
		}
	}

	public IDisposable MakeCurrent()
	{
		return MakeCurrent(OffscreenSurface);
	}

	public IDisposable MakeCurrent(EglSurface? surface)
	{
		if (IsLost)
		{
			throw new PlatformGraphicsContextLostException();
		}
		Monitor.Enter(_lock);
		bool flag = false;
		try
		{
			RestoreContext result = new RestoreContext(_egl, _disp.Handle, _lock);
			EglSurface eglSurface = surface ?? OffscreenSurface;
			_egl.MakeCurrent(_disp.Handle, IntPtr.Zero, IntPtr.Zero, IntPtr.Zero);
			if (!_egl.MakeCurrent(_disp.Handle, eglSurface?.DangerousGetHandle() ?? IntPtr.Zero, eglSurface?.DangerousGetHandle() ?? IntPtr.Zero, Context))
			{
				int error = _egl.GetError();
				if (error == 12302)
				{
					NotifyContextLost();
					throw new PlatformGraphicsContextLostException();
				}
				throw OpenGlException.GetFormattedEglException("eglMakeCurrent", error);
			}
			flag = true;
			return result;
		}
		finally
		{
			if (!flag)
			{
				Monitor.Exit(_lock);
			}
		}
	}

	public void NotifyContextLost()
	{
		_isLost = true;
		_disp.OnContextLost(this);
	}

	public IDisposable EnsureCurrent()
	{
		if (IsCurrent)
		{
			return Disposable.Empty;
		}
		return MakeCurrent();
	}

	public IDisposable EnsureLocked()
	{
		if (IsCurrent)
		{
			return Disposable.Empty;
		}
		Monitor.Enter(_lock);
		return Disposable.Create(delegate
		{
			Monitor.Exit(_lock);
		});
	}

	public bool IsSharedWith(IGlContext context)
	{
		EglContext eglContext = (EglContext)context;
		if (eglContext != this && eglContext._sharedWith != this && _sharedWith != context)
		{
			if (_sharedWith != null)
			{
				return _sharedWith == eglContext._sharedWith;
			}
			return false;
		}
		return true;
	}

	public IGlContext CreateSharedContext(IEnumerable<GlVersion>? preferredVersions = null)
	{
		return _disp.CreateContext(new EglContextOptions
		{
			ShareWith = (_sharedWith ?? this)
		});
	}

	public void Dispose()
	{
		if (_context == IntPtr.Zero)
		{
			return;
		}
		foreach (KeyValuePair<Type, object> item in _features.ToList())
		{
			if (item.Value is IDisposable disposable)
			{
				disposable.Dispose();
				_features.Remove(item.Key);
			}
		}
		_egl.DestroyContext(_disp.Handle, Context);
		OffscreenSurface?.Dispose();
		_context = IntPtr.Zero;
		_disp.OnContextDisposed(this);
		_disposeCallback?.Invoke();
	}

	public object? TryGetFeature(Type featureType)
	{
		if (_features.TryGetValue(featureType, out object value))
		{
			return value;
		}
		return null;
	}
}
