using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Win32.DirectX;
using MicroCom.Runtime;

namespace Avalonia.Win32.OpenGl.Angle;

internal class AngleExternalMemoryD3D11ExportedTexture2D : AngleExternalMemoryD3D11Texture2D, IGlExportableExternalImageTexture, IGlExternalImageTexture, IDisposable
{
	public IPlatformHandle Handle { get; }

	private static IPlatformHandle GetHandle(ID3D11Texture2D texture2D)
	{
		using IDXGIResource iDXGIResource = texture2D.QueryInterface<IDXGIResource>();
		return new PlatformHandle(iDXGIResource.SharedHandle, "D3D11TextureGlobalSharedHandle");
	}

	public AngleExternalMemoryD3D11ExportedTexture2D(EglContext context, ID3D11Texture2D texture2D, D3D11_TEXTURE2D_DESC desc, PlatformGraphicsExternalImageFormat format)
		: this(context, texture2D, GetHandle(texture2D), new PlatformGraphicsExternalImageProperties
		{
			Width = (int)desc.Width,
			Height = (int)desc.Height,
			Format = format
		})
	{
	}

	private AngleExternalMemoryD3D11ExportedTexture2D(EglContext context, ID3D11Texture2D texture2D, IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties)
		: base(context, texture2D, properties)
	{
		Handle = handle;
	}

	public IPlatformHandle GetHandle()
	{
		return Handle;
	}
}
