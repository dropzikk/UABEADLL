using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Avalonia.Reactive;

namespace Avalonia.OpenGL.Egl;

public class EglDisplay : IDisposable
{
	private readonly EglInterface _egl;

	private IntPtr _display;

	private readonly EglDisplayOptions _options;

	private EglConfigInfo _config;

	private bool _isLost;

	private readonly object _lock = new object();

	private readonly List<EglContext> _contexts = new List<EglContext>();

	public bool SupportsSharing { get; }

	public IntPtr Handle => _display;

	public IntPtr Config => _config.Config;

	internal bool SingleContext => !_options.SupportsMultipleContexts;

	public EglInterface EglInterface => _egl;

	protected virtual bool DisplayLockIsSharedWithContexts => false;

	internal object? ContextSharedSyncRoot
	{
		get
		{
			if (!DisplayLockIsSharedWithContexts)
			{
				return null;
			}
			return _lock;
		}
	}

	public bool IsLost
	{
		get
		{
			if (_isLost || _display == IntPtr.Zero)
			{
				return true;
			}
			Func<bool>? deviceLostCheckCallback = _options.DeviceLostCheckCallback;
			if (deviceLostCheckCallback != null && deviceLostCheckCallback())
			{
				return _isLost = true;
			}
			return false;
		}
	}

	public EglDisplay()
		: this(new EglDisplayCreationOptions
		{
			Egl = new EglInterface()
		})
	{
	}

	public EglDisplay(EglDisplayCreationOptions options)
		: this(EglDisplayUtils.CreateDisplay(options), options)
	{
	}

	public EglDisplay(IntPtr display, EglDisplayOptions options)
	{
		_egl = options.Egl ?? new EglInterface();
		SupportsSharing = options.SupportsContextSharing;
		_display = display;
		_options = options;
		if (_display == IntPtr.Zero)
		{
			throw new ArgumentException();
		}
		_config = EglDisplayUtils.InitializeAndGetConfig(_egl, display, options.GlVersions);
	}

	public EglContext CreateContext(EglContextOptions? options)
	{
		if (SingleContext && _contexts.Any())
		{
			throw new OpenGlException("This EGLDisplay can only have one active context");
		}
		if (options == null)
		{
			options = new EglContextOptions();
		}
		lock (_lock)
		{
			EglContext shareWith = options.ShareWith;
			if (shareWith != null && !SupportsSharing)
			{
				throw new NotSupportedException("Context sharing is not supported by this display");
			}
			EglSurface eglSurface = options.OffscreenSurface;
			if (eglSurface == null)
			{
				string? text = _egl.QueryString(Handle, 12373);
				if (text == null || !text.Contains("EGL_KHR_surfaceless_context"))
				{
					if ((_config.SurfaceType | 1) == 0)
					{
						throw new InvalidOperationException("Platform doesn't support EGL_KHR_surfaceless_context and PBUFFER surfaces");
					}
					IntPtr intPtr = _egl.CreatePBufferSurface(_display, Config, new int[5] { 12375, 1, 12374, 1, 12344 });
					if (intPtr == IntPtr.Zero)
					{
						throw OpenGlException.GetFormattedException("eglCreatePBufferSurface", _egl);
					}
					eglSurface = new EglSurface(this, intPtr);
				}
			}
			IntPtr intPtr2 = _egl.CreateContext(_display, Config, shareWith?.Context ?? IntPtr.Zero, _config.Attributes);
			if (intPtr2 == IntPtr.Zero)
			{
				OpenGlException formattedException = OpenGlException.GetFormattedException("eglCreateContext", _egl);
				eglSurface?.Dispose();
				throw formattedException;
			}
			EglContext eglContext = new EglContext(this, _egl, shareWith, intPtr2, eglSurface, _config.Version, _config.SampleCount, _config.StencilSize, options.DisposeCallback, options.ExtraFeatures ?? new Dictionary<Type, Func<EglContext, object>>());
			_contexts.Add(eglContext);
			return eglContext;
		}
	}

	public EglSurface CreateWindowSurface(IntPtr window)
	{
		if (window == IntPtr.Zero)
		{
			throw new OpenGlException($"Window {window} is invalid.");
		}
		using (Lock())
		{
			IntPtr intPtr = EglInterface.CreateWindowSurface(Handle, Config, window, new int[2] { 12344, 12344 });
			if (intPtr == IntPtr.Zero)
			{
				throw OpenGlException.GetFormattedException("eglCreateWindowSurface", EglInterface);
			}
			return new EglSurface(this, intPtr);
		}
	}

	public EglSurface CreatePBufferFromClientBuffer(int bufferType, IntPtr handle, int[] attribs)
	{
		using (Lock())
		{
			IntPtr intPtr = EglInterface.CreatePbufferFromClientBuffer(Handle, bufferType, handle, Config, attribs);
			if (intPtr == IntPtr.Zero)
			{
				throw OpenGlException.GetFormattedException("eglCreatePbufferFromClientBuffer", EglInterface);
			}
			return new EglSurface(this, intPtr);
		}
	}

	internal void OnContextLost(EglContext context)
	{
		if (_options.ContextLossIsDisplayLoss)
		{
			_isLost = true;
		}
	}

	internal void OnContextDisposed(EglContext context)
	{
		lock (_lock)
		{
			_contexts.Remove(context);
		}
	}

	public IDisposable Lock()
	{
		Monitor.Enter(_lock);
		return Disposable.Create(delegate
		{
			Monitor.Exit(_lock);
		});
	}

	public void Dispose()
	{
		lock (_lock)
		{
			foreach (EglContext context in _contexts)
			{
				context.Dispose();
			}
			_contexts.Clear();
			if (_display != IntPtr.Zero)
			{
				_egl.Terminate(_display);
			}
			_display = IntPtr.Zero;
			_options.DisposeCallback?.Invoke();
		}
	}
}
