using System;

namespace Avalonia.OpenGL;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
internal sealed class GlExtensionEntryPoint : Attribute
{
	public GlExtensionEntryPoint(string entry, string extension)
	{
	}

	public GlExtensionEntryPoint(string entry, string extension, GlProfileType profile)
	{
	}

	public static IntPtr GetProcAddress(Func<string, IntPtr> getProcAddress, GlInterface.GlContextInfo context, string entry, string extension, GlProfileType? profile = null)
	{
		if (profile.HasValue && profile != context.Version.Type)
		{
			return IntPtr.Zero;
		}
		if (!context.Extensions.Contains(extension))
		{
			return IntPtr.Zero;
		}
		return getProcAddress(entry);
	}
}
