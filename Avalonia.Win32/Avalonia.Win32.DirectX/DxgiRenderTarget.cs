using System;
using Avalonia.OpenGL.Egl;
using Avalonia.OpenGL.Surfaces;
using Avalonia.Win32.Interop;
using Avalonia.Win32.OpenGl.Angle;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal class DxgiRenderTarget : EglPlatformSurfaceRenderTargetBase
{
	public const uint DXGI_USAGE_RENDER_TARGET_OUTPUT = 32u;

	private readonly Guid ID3D11Texture2DGuid = Guid.Parse("6F15AAF2-D208-4E89-9AB4-489535D34F9C");

	private readonly EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo _window;

	private readonly DxgiConnection _connection;

	private readonly IDXGIDevice? _dxgiDevice;

	private readonly IDXGIFactory2? _dxgiFactory;

	private readonly IDXGISwapChain1? _swapChain;

	private readonly uint _flagsUsed;

	private IUnknown? _renderTexture;

	private UnmanagedMethods.RECT _clientRect;

	public unsafe DxgiRenderTarget(EglGlPlatformSurface.IEglWindowGlPlatformSurfaceInfo window, EglContext context, DxgiConnection connection)
		: base(context)
	{
		_window = window;
		_connection = connection;
		IUnknown unknown = MicroComRuntime.CreateProxyFor<IUnknown>(((AngleWin32EglDisplay)context.Display).GetDirect3DDevice(), ownsHandle: false);
		_dxgiDevice = unknown.QueryInterface<IDXGIDevice>();
		using (IDXGIAdapter iDXGIAdapter = _dxgiDevice.Adapter)
		{
			Guid guidFor = MicroComRuntime.GetGuidFor(typeof(IDXGIFactory2));
			_dxgiFactory = MicroComRuntime.CreateProxyFor<IDXGIFactory2>(iDXGIAdapter.GetParent(&guidFor), ownsHandle: true);
		}
		DXGI_SWAP_CHAIN_DESC1 dXGI_SWAP_CHAIN_DESC = new DXGI_SWAP_CHAIN_DESC1
		{
			Format = DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM,
			SampleDesc = 
			{
				Count = 1u,
				Quality = 0u
			},
			BufferUsage = 32u,
			AlphaMode = DXGI_ALPHA_MODE.DXGI_ALPHA_MODE_IGNORE,
			Width = (uint)_window.Size.Width,
			Height = (uint)_window.Size.Height,
			BufferCount = 2u,
			SwapEffect = DXGI_SWAP_EFFECT.DXGI_SWAP_EFFECT_FLIP_DISCARD
		};
		_flagsUsed = (dXGI_SWAP_CHAIN_DESC.Flags = 2048u);
		_swapChain = _dxgiFactory.CreateSwapChainForHwnd(_dxgiDevice, window.Handle, &dXGI_SWAP_CHAIN_DESC, null, null);
		UnmanagedMethods.GetClientRect(_window.Handle, out var lpRect);
		_clientRect = lpRect;
	}

	public unsafe override IGlPlatformSurfaceRenderingSession BeginDrawCore()
	{
		if (_swapChain == null)
		{
			throw new InvalidOperationException("No chain to draw on");
		}
		IDisposable contextLock = base.Context.EnsureCurrent();
		EglSurface surface = null;
		IDisposable transaction = null;
		bool flag = false;
		try
		{
			UnmanagedMethods.GetClientRect(_window.Handle, out var lpRect);
			if (!RectsEqual(in lpRect, in _clientRect))
			{
				_clientRect = lpRect;
				if (_renderTexture != null)
				{
					_renderTexture.Dispose();
					_renderTexture = null;
				}
				_swapChain.ResizeBuffers(2, (ushort)(lpRect.right - lpRect.left), (ushort)(lpRect.bottom - lpRect.top), DXGI_FORMAT.DXGI_FORMAT_B8G8R8A8_UNORM, (ushort)_flagsUsed);
			}
			PixelSize size = _window.Size;
			IUnknown unknown = _renderTexture;
			if (unknown == null)
			{
				Guid iD3D11Texture2DGuid = ID3D11Texture2DGuid;
				unknown = MicroComRuntime.CreateProxyFor<IUnknown>(_swapChain.GetBuffer(0, &iD3D11Texture2DGuid), ownsHandle: true);
			}
			_renderTexture = unknown;
			surface = ((AngleWin32EglDisplay)base.Context.Display).WrapDirect3D11Texture(_renderTexture.GetNativeIntPtr(), 0, 0, size.Width, size.Height);
			IGlPlatformSurfaceRenderingSession result = BeginDraw(surface, _window.Size, _window.Scaling, delegate
			{
				_swapChain.Present(0, 0);
				surface.Dispose();
				transaction?.Dispose();
				contextLock?.Dispose();
			}, isYFlipped: true);
			flag = true;
			return result;
		}
		finally
		{
			if (!flag)
			{
				surface?.Dispose();
				if (_renderTexture != null)
				{
					_renderTexture.Dispose();
					_renderTexture = null;
				}
				transaction?.Dispose();
				contextLock.Dispose();
			}
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		_dxgiDevice?.Dispose();
		_dxgiFactory?.Dispose();
		_swapChain?.Dispose();
		_renderTexture?.Dispose();
	}

	internal static bool RectsEqual(in UnmanagedMethods.RECT l, in UnmanagedMethods.RECT r)
	{
		if (l.left == r.left && l.top == r.top && l.right == r.right)
		{
			return l.bottom == r.bottom;
		}
		return false;
	}
}
