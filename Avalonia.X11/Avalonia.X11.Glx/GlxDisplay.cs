using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.OpenGL;

namespace Avalonia.X11.Glx;

internal class GlxDisplay
{
	private readonly X11Info _x11;

	private readonly GlVersion[] _probeProfiles;

	private readonly IntPtr _fbconfig;

	private unsafe readonly XVisualInfo* _visual;

	private string[] _displayExtensions;

	private GlVersion? _version;

	public unsafe XVisualInfo* VisualInfo => _visual;

	public GlxContext DeferredContext { get; }

	public GlxInterface Glx { get; } = new GlxInterface();

	public unsafe GlxDisplay(X11Info x11, IList<GlVersion> probeProfiles)
	{
		_x11 = x11;
		_probeProfiles = probeProfiles.ToArray();
		_displayExtensions = Glx.GetExtensions(_x11.DeferredDisplay);
		int[] array = new int[20]
		{
			32786, 1, 32785, 1, 32784, 5, 5, 1, 8, 8,
			9, 8, 10, 8, 11, 8, 12, 1, 13, 8
		};
		int sampleCount = 0;
		int stencilSize = 0;
		int[][] array2 = new int[1][] { array };
		foreach (int[] attrib_list in array2)
		{
			int nelements;
			IntPtr* ptr = Glx.ChooseFBConfig(_x11.DeferredDisplay, x11.DefaultScreen, attrib_list, out nelements);
			for (int j = 0; j < nelements; j++)
			{
				XVisualInfo* visualFromFBConfig = Glx.GetVisualFromFBConfig(_x11.DeferredDisplay, ptr[j]);
				if (_fbconfig == IntPtr.Zero || visualFromFBConfig->depth == 32)
				{
					_fbconfig = ptr[j];
					_visual = visualFromFBConfig;
					if (visualFromFBConfig->depth == 32)
					{
						break;
					}
				}
			}
			if (_fbconfig != IntPtr.Zero)
			{
				break;
			}
		}
		if (_fbconfig == IntPtr.Zero)
		{
			throw new OpenGlException("Unable to choose FBConfig");
		}
		if (_visual == null)
		{
			throw new OpenGlException("Unable to get visual info from FBConfig");
		}
		if (Glx.GetFBConfigAttrib(_x11.DeferredDisplay, _fbconfig, 100001, out var value) == 0)
		{
			sampleCount = value;
		}
		if (Glx.GetFBConfigAttrib(_x11.DeferredDisplay, _fbconfig, 13, out var value2) == 0)
		{
			stencilSize = value2;
		}
		int[] attrib_list2 = new int[5] { 32833, 1, 32832, 1, 0 };
		Glx.CreatePbuffer(_x11.DeferredDisplay, _fbconfig, attrib_list2);
		Glx.CreatePbuffer(_x11.DeferredDisplay, _fbconfig, attrib_list2);
		XLib.XFlush(_x11.DeferredDisplay);
		DeferredContext = CreateContext(CreatePBuffer(), null, sampleCount, stencilSize, ownsPBuffer: true);
		using (DeferredContext.MakeCurrent())
		{
			GlInterface glInterface = DeferredContext.GlInterface;
			if (glInterface.Version == null)
			{
				throw new OpenGlException("GL version string is null, aborting");
			}
			if (glInterface.Renderer == null)
			{
				throw new OpenGlException("GL renderer string is null, aborting");
			}
			if (!(Environment.GetEnvironmentVariable("AVALONIA_GLX_IGNORE_RENDERER_BLACKLIST") != "1"))
			{
				return;
			}
			IList<string> glxRendererBlacklist = (AvaloniaLocator.Current.GetService<X11PlatformOptions>() ?? new X11PlatformOptions()).GlxRendererBlacklist;
			if (glxRendererBlacklist == null)
			{
				return;
			}
			foreach (string item in glxRendererBlacklist)
			{
				if (glInterface.Renderer.Contains(item))
				{
					throw new OpenGlException($"Renderer '{glInterface.Renderer}' is blacklisted by '{item}'");
				}
			}
		}
	}

	private IntPtr CreatePBuffer()
	{
		return Glx.CreatePbuffer(_x11.DeferredDisplay, _fbconfig, new int[5] { 32833, 1, 32832, 1, 0 });
	}

	public GlxContext CreateContext()
	{
		return CreateContext(CreatePBuffer(), null, DeferredContext.SampleCount, DeferredContext.StencilSize, ownsPBuffer: true);
	}

	public GlxContext CreateContext(IGlContext share)
	{
		return CreateContext(CreatePBuffer(), share, share.SampleCount, share.StencilSize, ownsPBuffer: true);
	}

	private GlxContext CreateContext(IntPtr defaultXid, IGlContext share, int sampleCount, int stencilSize, bool ownsPBuffer)
	{
		IntPtr sharelist = ((GlxContext)share)?.Handle ?? IntPtr.Zero;
		IntPtr handle = default(IntPtr);
		GlxContext glxContext = null;
		if (_version.HasValue)
		{
			glxContext = Create(_version.Value);
		}
		GlVersion[] probeProfiles = _probeProfiles;
		for (int i = 0; i < probeProfiles.Length; i++)
		{
			GlVersion glVersion = probeProfiles[i];
			if (glVersion.Type != GlProfileType.OpenGLES || _displayExtensions.Contains("GLX_EXT_create_context_es2_profile"))
			{
				glxContext = Create(glVersion);
				if (glxContext != null)
				{
					_version = glVersion;
					break;
				}
			}
		}
		if (glxContext != null)
		{
			return glxContext;
		}
		throw new OpenGlException("Unable to create direct GLX context");
		GlxContext Create(GlVersion profile)
		{
			int num = 1;
			if (profile.Type == GlProfileType.OpenGLES)
			{
				num = 4;
			}
			int[] obj = new int[7] { 8337, 0, 8338, 0, 37158, 0, 0 };
			obj[1] = profile.Major;
			obj[3] = profile.Minor;
			obj[5] = num;
			int[] attribs = obj;
			try
			{
				handle = Glx.CreateContextAttribsARB(_x11.DeferredDisplay, _fbconfig, sharelist, direct: true, attribs);
				if (handle != IntPtr.Zero)
				{
					_version = profile;
					return new GlxContext(new GlxInterface(), handle, this, (GlxContext)share, profile, sampleCount, stencilSize, _x11, defaultXid, ownsPBuffer);
				}
			}
			catch
			{
				return null;
			}
			return null;
		}
	}

	public void SwapBuffers(IntPtr xid)
	{
		Glx.SwapBuffers(_x11.DeferredDisplay, xid);
	}
}
