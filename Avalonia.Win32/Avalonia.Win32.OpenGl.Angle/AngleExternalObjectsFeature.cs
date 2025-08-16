using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;
using Avalonia.Win32.DirectX;
using MicroCom.Runtime;

namespace Avalonia.Win32.OpenGl.Angle;

internal class AngleExternalObjectsFeature : IGlContextExternalObjectsFeature, IDisposable
{
	private readonly EglContext _context;

	private readonly ID3D11Device _device;

	private readonly ID3D11Device1 _device1;

	public IReadOnlyList<string> SupportedImportableExternalImageTypes { get; } = new string[2] { "D3D11TextureGlobalSharedHandle", "D3D11TextureNtHandle" };

	public IReadOnlyList<string> SupportedExportableExternalImageTypes => SupportedImportableExternalImageTypes;

	public IReadOnlyList<string> SupportedImportableExternalSemaphoreTypes => Array.Empty<string>();

	public IReadOnlyList<string> SupportedExportableExternalSemaphoreTypes => Array.Empty<string>();

	public byte[]? DeviceLuid { get; }

	public byte[]? DeviceUuid => null;

	public AngleExternalObjectsFeature(EglContext context)
	{
		_context = context;
		AngleWin32EglDisplay angleWin32EglDisplay = (AngleWin32EglDisplay)context.Display;
		_device = MicroComRuntime.CreateProxyFor<ID3D11Device>(angleWin32EglDisplay.GetDirect3DDevice(), ownsHandle: false).CloneReference();
		_device1 = _device.QueryInterface<ID3D11Device1>();
		using IDXGIDevice iDXGIDevice = _device.QueryInterface<IDXGIDevice>();
		using IDXGIAdapter iDXGIAdapter = iDXGIDevice.Adapter;
		DeviceLuid = BitConverter.GetBytes(iDXGIAdapter.Desc.AdapterLuid);
	}

	public IReadOnlyList<PlatformGraphicsExternalImageFormat> GetSupportedFormatsForExternalMemoryType(string type)
	{
		return new PlatformGraphicsExternalImageFormat[1];
	}

	public unsafe IGlExportableExternalImageTexture CreateImage(string type, PixelSize size, PlatformGraphicsExternalImageFormat format)
	{
		if (format != 0)
		{
			throw new NotSupportedException("Unsupported external memory format");
		}
		using (_context.EnsureCurrent())
		{
			DXGI_FORMAT format2 = DXGI_FORMAT.DXGI_FORMAT_R8G8B8A8_UNORM;
			D3D11_TEXTURE2D_DESC d3D11_TEXTURE2D_DESC = default(D3D11_TEXTURE2D_DESC);
			d3D11_TEXTURE2D_DESC.Format = format2;
			d3D11_TEXTURE2D_DESC.Width = (uint)size.Width;
			d3D11_TEXTURE2D_DESC.Height = (uint)size.Height;
			d3D11_TEXTURE2D_DESC.ArraySize = 1u;
			d3D11_TEXTURE2D_DESC.MipLevels = 1u;
			d3D11_TEXTURE2D_DESC.SampleDesc = new DXGI_SAMPLE_DESC
			{
				Count = 1u,
				Quality = 0u
			};
			d3D11_TEXTURE2D_DESC.Usage = D3D11_USAGE.D3D11_USAGE_DEFAULT;
			d3D11_TEXTURE2D_DESC.CPUAccessFlags = 0u;
			d3D11_TEXTURE2D_DESC.MiscFlags = D3D11_RESOURCE_MISC_FLAG.D3D11_RESOURCE_MISC_SHARED_KEYEDMUTEX;
			d3D11_TEXTURE2D_DESC.BindFlags = D3D11_BIND_FLAG.D3D11_BIND_SHADER_RESOURCE | D3D11_BIND_FLAG.D3D11_BIND_RENDER_TARGET;
			D3D11_TEXTURE2D_DESC desc = d3D11_TEXTURE2D_DESC;
			using ID3D11Texture2D texture2D = _device.CreateTexture2D(&desc, IntPtr.Zero);
			return new AngleExternalMemoryD3D11ExportedTexture2D(_context, texture2D, desc, format);
		}
	}

	public IGlExportableExternalImageTexture CreateSemaphore(string type)
	{
		throw new NotSupportedException();
	}

	public unsafe IGlExternalImageTexture ImportImage(IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties)
	{
		if (!SupportedImportableExternalImageTypes.Contains<string>(handle.HandleDescriptor))
		{
			throw new NotSupportedException("Unsupported external memory type");
		}
		using (_context.EnsureCurrent())
		{
			Guid guidFor = MicroComRuntime.GetGuidFor(typeof(ID3D11Texture2D));
			using IUnknown unknown = ((handle.HandleDescriptor == "D3D11TextureGlobalSharedHandle") ? _device.OpenSharedResource(handle.Handle, &guidFor) : _device1.OpenSharedResource1(handle.Handle, &guidFor));
			using ID3D11Texture2D texture2D = unknown.QueryInterface<ID3D11Texture2D>();
			return new AngleExternalMemoryD3D11Texture2D(_context, texture2D, properties);
		}
	}

	public IGlExternalSemaphore ImportSemaphore(IPlatformHandle handle)
	{
		throw new NotSupportedException();
	}

	public CompositionGpuImportedImageSynchronizationCapabilities GetSynchronizationCapabilities(string imageHandleType)
	{
		if (imageHandleType == "D3D11TextureGlobalSharedHandle" || imageHandleType == "D3D11TextureNtHandle")
		{
			return CompositionGpuImportedImageSynchronizationCapabilities.KeyedMutex;
		}
		return (CompositionGpuImportedImageSynchronizationCapabilities)0;
	}

	public void Dispose()
	{
		_device.Dispose();
	}
}
