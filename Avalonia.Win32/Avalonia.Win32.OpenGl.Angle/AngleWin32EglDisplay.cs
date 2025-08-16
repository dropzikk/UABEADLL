using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Angle;
using Avalonia.OpenGL.Egl;
using Avalonia.Win32.DirectX;
using MicroCom.Runtime;

namespace Avalonia.Win32.OpenGl.Angle;

internal class AngleWin32EglDisplay : EglDisplay
{
	protected override bool DisplayLockIsSharedWithContexts => true;

	public AngleOptions.PlatformApi PlatformApi { get; }

	public static AngleWin32EglDisplay CreateD3D9Display(EglInterface egl)
	{
		return new AngleWin32EglDisplay(egl.GetPlatformDisplayExt(12802, IntPtr.Zero, new int[3] { 12803, 12807, 12344 }), new EglDisplayOptions
		{
			Egl = egl,
			ContextLossIsDisplayLoss = true,
			GlVersions = AvaloniaLocator.Current.GetService<AngleOptions>()?.GlProfiles
		}, AngleOptions.PlatformApi.DirectX9);
	}

	public static AngleWin32EglDisplay CreateSharedD3D11Display(EglInterface egl)
	{
		return new AngleWin32EglDisplay(egl.GetPlatformDisplayExt(12802, IntPtr.Zero, new int[3] { 12803, 12808, 12344 }), new EglDisplayOptions
		{
			Egl = egl,
			ContextLossIsDisplayLoss = true,
			GlVersions = AvaloniaLocator.Current.GetService<AngleOptions>()?.GlProfiles
		}, AngleOptions.PlatformApi.DirectX11);
	}

	public unsafe static AngleWin32EglDisplay CreateD3D11Display(Win32AngleEglInterface egl, bool preferDiscreteAdapter = false)
	{
		D3D_FEATURE_LEVEL[] array = new D3D_FEATURE_LEVEL[7]
		{
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_1,
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_11_0,
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_1,
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_10_0,
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_3,
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_2,
			D3D_FEATURE_LEVEL.D3D_FEATURE_LEVEL_9_1
		};
		Guid riid = MicroComRuntime.GetGuidFor(typeof(IDXGIFactory1));
		DirectXUnmanagedMethods.CreateDXGIFactory1(ref riid, out var ppFactory);
		IDXGIAdapter1 iDXGIAdapter = null;
		if (ppFactory != null)
		{
			using IDXGIFactory1 iDXGIFactory = MicroComRuntime.CreateProxyFor<IDXGIFactory1>(ppFactory, ownsHandle: true);
			void* pObject = null;
			if (preferDiscreteAdapter)
			{
				ushort num = 0;
				List<(IDXGIAdapter1, string)> list = new List<(IDXGIAdapter1, string)>();
				while (iDXGIFactory.EnumAdapters1(num, &pObject) == 0)
				{
					IDXGIAdapter1 iDXGIAdapter2 = MicroComRuntime.CreateProxyFor<IDXGIAdapter1>(pObject, ownsHandle: true);
					DXGI_ADAPTER_DESC1 desc = iDXGIAdapter2.Desc1;
					string item = Marshal.PtrToStringUni(new IntPtr(desc.Description)).ToLowerInvariant();
					list.Add((iDXGIAdapter2, item));
					num++;
				}
				if (list.Count == 0)
				{
					throw new OpenGlException("No adapters found");
				}
				iDXGIAdapter = list.OrderByDescending<(IDXGIAdapter1, string), int>(((IDXGIAdapter1 adapter, string name) x) => (!x.name.Contains("nvidia")) ? (x.name.Contains("amd") ? 1 : 0) : 2).First().Item1.CloneReference();
				foreach (var item2 in list)
				{
					item2.Item1.Dispose();
				}
			}
			else
			{
				if (iDXGIFactory.EnumAdapters1(0, &pObject) != 0)
				{
					throw new OpenGlException("No adapters found");
				}
				iDXGIAdapter = MicroComRuntime.CreateProxyFor<IDXGIAdapter1>(pObject, ownsHandle: true);
			}
		}
		IntPtr ppDevice;
		using (iDXGIAdapter)
		{
			DirectXUnmanagedMethods.D3D11CreateDevice(iDXGIAdapter?.GetNativeIntPtr() ?? IntPtr.Zero, D3D_DRIVER_TYPE.D3D_DRIVER_TYPE_UNKNOWN, IntPtr.Zero, 0u, array, (uint)array.Length, 7u, out ppDevice, out var _, null);
		}
		if (ppDevice == IntPtr.Zero)
		{
			throw new Win32Exception("Unable to create D3D11 Device");
		}
		ID3D11Device d3dDevice = MicroComRuntime.CreateProxyFor<ID3D11Device>(ppDevice, ownsHandle: true);
		IntPtr angleDevice = IntPtr.Zero;
		IntPtr intPtr = IntPtr.Zero;
		bool flag = false;
		try
		{
			angleDevice = egl.CreateDeviceANGLE(13217, ppDevice, null);
			if (angleDevice == IntPtr.Zero)
			{
				throw OpenGlException.GetFormattedException("eglCreateDeviceANGLE", egl);
			}
			intPtr = egl.GetPlatformDisplayExt(12607, angleDevice, null);
			if (intPtr == IntPtr.Zero)
			{
				throw OpenGlException.GetFormattedException("eglGetPlatformDisplayEXT", egl);
			}
			AngleWin32EglDisplay result = new AngleWin32EglDisplay(intPtr, new EglDisplayOptions
			{
				DisposeCallback = Cleanup,
				Egl = egl,
				ContextLossIsDisplayLoss = true,
				DeviceLostCheckCallback = () => d3dDevice.DeviceRemovedReason != 0,
				GlVersions = AvaloniaLocator.Current.GetService<AngleOptions>()?.GlProfiles
			}, AngleOptions.PlatformApi.DirectX11);
			flag = true;
			return result;
		}
		finally
		{
			if (!flag)
			{
				if (intPtr != IntPtr.Zero)
				{
					egl.Terminate(intPtr);
				}
				Cleanup();
			}
		}
		void Cleanup()
		{
			if (angleDevice != IntPtr.Zero)
			{
				egl.ReleaseDeviceANGLE(angleDevice);
			}
			d3dDevice.Dispose();
		}
	}

	private AngleWin32EglDisplay(IntPtr display, EglDisplayOptions options, AngleOptions.PlatformApi platformApi)
		: base(display, options)
	{
		PlatformApi = platformApi;
	}

	public IntPtr GetDirect3DDevice()
	{
		if (!base.EglInterface.QueryDisplayAttribExt(base.Handle, 12844, out var res))
		{
			throw new OpenGlException("Unable to get EGL_DEVICE_EXT");
		}
		if (!base.EglInterface.QueryDeviceAttribExt(res, (PlatformApi == AngleOptions.PlatformApi.DirectX9) ? 13216 : 13217, out var res2))
		{
			throw new OpenGlException("Unable to get EGL_D3D9_DEVICE_ANGLE");
		}
		return res2;
	}

	public EglSurface WrapDirect3D11Texture(IntPtr handle)
	{
		if (PlatformApi != AngleOptions.PlatformApi.DirectX11)
		{
			throw new InvalidOperationException("Current platform API is " + PlatformApi);
		}
		return CreatePBufferFromClientBuffer(13219, handle, new int[2] { 12344, 12344 });
	}

	public EglSurface WrapDirect3D11Texture(IntPtr handle, int offsetX, int offsetY, int width, int height)
	{
		if (PlatformApi != AngleOptions.PlatformApi.DirectX11)
		{
			throw new InvalidOperationException("Current platform API is " + PlatformApi);
		}
		int[] obj = new int[11]
		{
			12375, 0, 12374, 0, 13222, 1, 13456, 0, 13457, 0,
			12344
		};
		obj[1] = width;
		obj[3] = height;
		obj[7] = offsetX;
		obj[9] = offsetY;
		return CreatePBufferFromClientBuffer(13219, handle, obj);
	}
}
