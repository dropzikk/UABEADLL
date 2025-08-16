using System;
using System.Collections.Generic;
using System.Threading;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Features;
using Avalonia.Platform;
using Avalonia.Reactive;

namespace Avalonia.X11.Glx;

internal class GlxContext : IGlContext, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	private class RestoreContext : IDisposable
	{
		private GlxInterface _glx;

		private IntPtr _defaultDisplay;

		private readonly object _l;

		private IntPtr _display;

		private IntPtr _context;

		private IntPtr _read;

		private IntPtr _draw;

		public RestoreContext(GlxInterface glx, IntPtr defaultDisplay, object l)
		{
			_glx = glx;
			_defaultDisplay = defaultDisplay;
			_l = l;
			_display = _glx.GetCurrentDisplay();
			_context = _glx.GetCurrentContext();
			_read = _glx.GetCurrentReadDrawable();
			_draw = _glx.GetCurrentDrawable();
		}

		public void Dispose()
		{
			IntPtr display = ((_display == IntPtr.Zero) ? _defaultDisplay : _display);
			_glx.MakeContextCurrent(display, _draw, _read, _context);
			Monitor.Exit(_l);
		}
	}

	private readonly GlxContext _sharedWith;

	private readonly X11Info _x11;

	private readonly IntPtr _defaultXid;

	private readonly bool _ownsPBuffer;

	private readonly object _lock = new object();

	private ExternalObjectsOpenGlExtensionFeature? _externalObjects;

	public IntPtr Handle { get; }

	public GlxInterface Glx { get; }

	public GlxDisplay Display { get; }

	public GlVersion Version { get; }

	public GlInterface GlInterface { get; }

	public int SampleCount { get; }

	public int StencilSize { get; }

	public bool IsLost => false;

	public bool CanCreateSharedContext => true;

	public bool IsCurrent => Glx.GetCurrentContext() == Handle;

	public GlxContext(GlxInterface glx, IntPtr handle, GlxDisplay display, GlxContext sharedWith, GlVersion version, int sampleCount, int stencilSize, X11Info x11, IntPtr defaultXid, bool ownsPBuffer)
	{
		Handle = handle;
		Glx = glx;
		_sharedWith = sharedWith;
		_x11 = x11;
		_defaultXid = defaultXid;
		_ownsPBuffer = ownsPBuffer;
		Display = display;
		Version = version;
		SampleCount = sampleCount;
		StencilSize = stencilSize;
		using (MakeCurrent())
		{
			GlInterface = new GlInterface(version, GlxInterface.SafeGetProcAddress);
			_externalObjects = ExternalObjectsOpenGlExtensionFeature.TryCreate(this);
		}
	}

	public IDisposable MakeCurrent()
	{
		return MakeCurrent(_defaultXid);
	}

	public IDisposable EnsureCurrent()
	{
		if (IsCurrent)
		{
			return Disposable.Empty;
		}
		return MakeCurrent();
	}

	public bool IsSharedWith(IGlContext context)
	{
		GlxContext glxContext = (GlxContext)context;
		if (glxContext != this && glxContext._sharedWith != this && _sharedWith != context)
		{
			if (_sharedWith != null)
			{
				return _sharedWith == glxContext._sharedWith;
			}
			return false;
		}
		return true;
	}

	public IGlContext CreateSharedContext(IEnumerable<GlVersion> preferredVersions = null)
	{
		return Display.CreateContext(_sharedWith ?? this);
	}

	public IDisposable MakeCurrent(IntPtr xid)
	{
		Monitor.Enter(_lock);
		bool flag = false;
		try
		{
			RestoreContext result = new RestoreContext(Glx, _x11.DeferredDisplay, _lock);
			if (!Glx.MakeContextCurrent(_x11.DeferredDisplay, xid, xid, Handle))
			{
				throw new OpenGlException("glXMakeContextCurrent failed ");
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

	public void Dispose()
	{
		Glx.DestroyContext(_x11.DeferredDisplay, Handle);
		if (_ownsPBuffer)
		{
			Glx.DestroyPbuffer(_x11.DeferredDisplay, _defaultXid);
		}
	}

	public object TryGetFeature(Type featureType)
	{
		if (featureType == typeof(IGlContextExternalObjectsFeature))
		{
			return _externalObjects;
		}
		return null;
	}
}
