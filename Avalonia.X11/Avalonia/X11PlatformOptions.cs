using System;
using System.Collections.Generic;
using System.Reflection;
using Avalonia.OpenGL;

namespace Avalonia;

public class X11PlatformOptions
{
	public IReadOnlyList<X11RenderingMode> RenderingMode { get; set; } = new X11RenderingMode[2]
	{
		X11RenderingMode.Glx,
		X11RenderingMode.Software
	};

	public bool OverlayPopups { get; set; }

	public bool UseDBusMenu { get; set; } = true;

	public bool UseDBusFilePicker { get; set; } = true;

	public bool? EnableIme { get; set; }

	public bool EnableSessionManagement { get; set; } = Environment.GetEnvironmentVariable("AVALONIA_X11_USE_SESSION_MANAGEMENT") != "0";

	public IList<GlVersion> GlProfiles { get; set; } = new List<GlVersion>
	{
		new GlVersion(GlProfileType.OpenGL, 4, 0),
		new GlVersion(GlProfileType.OpenGL, 3, 2),
		new GlVersion(GlProfileType.OpenGL, 3, 0),
		new GlVersion(GlProfileType.OpenGLES, 3, 2),
		new GlVersion(GlProfileType.OpenGLES, 3, 0),
		new GlVersion(GlProfileType.OpenGLES, 2, 0)
	};

	public IList<string> GlxRendererBlacklist { get; set; } = new List<string> { "llvmpipe" };

	public string WmClass { get; set; }

	public bool? EnableMultiTouch { get; set; } = true;

	public X11PlatformOptions()
	{
		try
		{
			WmClass = Assembly.GetEntryAssembly()?.GetName()?.Name;
		}
		catch
		{
		}
	}
}
