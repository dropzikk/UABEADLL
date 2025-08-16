using System;
using System.Collections.Generic;
using System.Linq;

namespace Avalonia.OpenGL.Egl;

internal static class EglDisplayUtils
{
	public static IntPtr CreateDisplay(EglDisplayCreationOptions options)
	{
		EglInterface eglInterface = options.Egl ?? new EglInterface();
		IntPtr intPtr = IntPtr.Zero;
		if (!options.PlatformType.HasValue)
		{
			if (intPtr == IntPtr.Zero)
			{
				intPtr = eglInterface.GetDisplay(IntPtr.Zero);
			}
		}
		else
		{
			if (!eglInterface.IsGetPlatformDisplayExtAvailable)
			{
				throw new OpenGlException("eglGetPlatformDisplayEXT is not supported by libegl");
			}
			intPtr = eglInterface.GetPlatformDisplayExt(options.PlatformType.Value, options.PlatformDisplay, options.PlatformDisplayAttrs);
		}
		if (intPtr == IntPtr.Zero)
		{
			throw OpenGlException.GetFormattedException("eglGetDisplay", eglInterface);
		}
		return intPtr;
	}

	public static EglConfigInfo InitializeAndGetConfig(EglInterface egl, IntPtr display, IEnumerable<GlVersion>? versions)
	{
		if (!egl.Initialize(display, out var _, out var _))
		{
			throw OpenGlException.GetFormattedException("eglInitialize", egl);
		}
		if (versions == null)
		{
			versions = new GlVersion[2]
			{
				new GlVersion(GlProfileType.OpenGLES, 3, 0),
				new GlVersion(GlProfileType.OpenGLES, 2, 0)
			};
		}
		foreach (var item in versions.Where((GlVersion x) => x.Type == GlProfileType.OpenGLES).Select(delegate(GlVersion x)
		{
			int renderableTypeBit = 64;
			switch (x.Major)
			{
			case 2:
				renderableTypeBit = 4;
				break;
			case 1:
				renderableTypeBit = 1;
				break;
			}
			int[] obj = new int[5] { 12440, 0, 12539, 0, 12344 };
			obj[1] = x.Major;
			obj[3] = x.Minor;
			return new
			{
				Attributes = obj,
				Api = 12448,
				RenderableTypeBit = renderableTypeBit,
				Version = x
			};
		}))
		{
			if (!egl.BindApi(item.Api))
			{
				continue;
			}
			int[] array = new int[2] { 5, 4 };
			foreach (int num in array)
			{
				int[] array2 = new int[3] { 8, 1, 0 };
				foreach (int num2 in array2)
				{
					int[] array3 = new int[3] { 8, 1, 0 };
					foreach (int num3 in array3)
					{
						int[] obj2 = new int[17]
						{
							12339, 0, 12352, 0, 12324, 8, 12323, 8, 12322, 8,
							12321, 8, 12326, 0, 12325, 0, 12344
						};
						obj2[1] = num;
						obj2[3] = item.RenderableTypeBit;
						obj2[13] = num2;
						obj2[15] = num3;
						int[] attribs = obj2;
						if (egl.ChooseConfig(display, attribs, out var surfaceConfig, 1, out var choosenConfig) && choosenConfig != 0)
						{
							egl.GetConfigAttrib(display, surfaceConfig, 12337, out var rv);
							egl.GetConfigAttrib(display, surfaceConfig, 12326, out var rv2);
							return new EglConfigInfo(surfaceConfig, item.Version, num, item.Attributes, rv, rv2);
						}
					}
				}
			}
		}
		throw new OpenGlException("No suitable EGL config was found");
	}
}
