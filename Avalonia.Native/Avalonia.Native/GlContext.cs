using System;
using System.Collections.Generic;
using Avalonia.Native.Interop;
using Avalonia.OpenGL;
using Avalonia.Platform;

namespace Avalonia.Native;

internal class GlContext : IGlContext, IPlatformGraphicsContext, IDisposable, IOptionalFeatureProvider
{
	private readonly GlDisplay _display;

	private readonly GlContext _sharedWith;

	public IAvnGlContext Context { get; private set; }

	public GlVersion Version { get; }

	public GlInterface GlInterface => _display.GlInterface;

	public int SampleCount => _display.SampleCount;

	public int StencilSize => _display.StencilSize;

	public bool IsLost => Context == null;

	public bool CanCreateSharedContext => true;

	public GlContext(GlDisplay display, GlContext sharedWith, IAvnGlContext context, GlVersion version)
	{
		_display = display;
		_sharedWith = sharedWith;
		Context = context;
		Version = version;
	}

	public IDisposable MakeCurrent()
	{
		if (IsLost)
		{
			throw new PlatformGraphicsContextLostException();
		}
		return Context.MakeCurrent();
	}

	public IDisposable EnsureCurrent()
	{
		return MakeCurrent();
	}

	public bool IsSharedWith(IGlContext context)
	{
		GlContext glContext = (GlContext)context;
		if (glContext != this && glContext._sharedWith != this && _sharedWith != context)
		{
			if (_sharedWith != null)
			{
				return _sharedWith == glContext._sharedWith;
			}
			return false;
		}
		return true;
	}

	public IGlContext CreateSharedContext(IEnumerable<GlVersion> preferredVersions = null)
	{
		return _display.CreateSharedContext(_sharedWith ?? this);
	}

	public void Dispose()
	{
		Context.Dispose();
		Context = null;
	}

	public object TryGetFeature(Type featureType)
	{
		return null;
	}
}
