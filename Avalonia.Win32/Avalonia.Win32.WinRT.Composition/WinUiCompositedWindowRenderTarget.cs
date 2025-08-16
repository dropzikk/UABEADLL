using System;
using Avalonia.Platform;
using Avalonia.Win32.DirectX;
using Avalonia.Win32.Interop;
using MicroCom.Runtime;

namespace Avalonia.Win32.WinRT.Composition;

internal class WinUiCompositedWindowRenderTarget : IDirect3D11TextureRenderTarget, IDisposable
{
	private class Session : IDirect3D11TextureRenderTargetRenderSession, IDisposable
	{
		private readonly IDisposable _transaction;

		private readonly PixelSize _size;

		private readonly PixelPoint _offset;

		private readonly double _scaling;

		private readonly ICompositionDrawingSurfaceInterop _surfaceInterop;

		private readonly IUnknown _texture;

		public IntPtr D3D11Texture2D => _texture.GetNativeIntPtr();

		public PixelSize Size => _size;

		public PixelPoint Offset => _offset;

		public double Scaling => _scaling;

		public Session(ICompositionDrawingSurfaceInterop surfaceInterop, IUnknown texture, IDisposable transaction, PixelSize size, PixelPoint offset, double scaling)
		{
			_transaction = transaction;
			_size = size;
			_offset = offset;
			_scaling = scaling;
			_surfaceInterop = surfaceInterop.CloneReference();
			_texture = texture.CloneReference();
		}

		public void Dispose()
		{
			try
			{
				_texture.Dispose();
				_surfaceInterop.EndDraw();
				_surfaceInterop.Dispose();
			}
			finally
			{
				_transaction.Dispose();
			}
		}
	}

	private static readonly Guid IID_ID3D11Texture2D = Guid.Parse("6f15aaf2-d208-4e89-9ab4-489535d34f9c");

	private readonly IPlatformGraphicsContext _context;

	private readonly WinUiCompositedWindow _window;

	private readonly IUnknown _d3dDevice;

	private readonly ICompositor _compositor;

	private readonly ICompositorInterop _interop;

	private readonly ICompositionGraphicsDevice _compositionDevice;

	private readonly ICompositionGraphicsDevice2 _compositionDevice2;

	private readonly ICompositionSurface _surface;

	private PixelSize _size;

	private bool _lost;

	private readonly ICompositionDrawingSurfaceInterop _surfaceInterop;

	private readonly ICompositionDrawingSurface _drawingSurface;

	public bool IsCorrupted
	{
		get
		{
			if (!_context.IsLost)
			{
				return _lost;
			}
			return true;
		}
	}

	public WinUiCompositedWindowRenderTarget(IPlatformGraphicsContext context, WinUiCompositedWindow window, IntPtr device, ICompositor compositor)
	{
		_context = context;
		_window = window;
		try
		{
			_d3dDevice = MicroComRuntime.CreateProxyFor<IUnknown>(device, ownsHandle: false).CloneReference();
			_compositor = compositor.CloneReference();
			_interop = compositor.QueryInterface<ICompositorInterop>();
			_compositionDevice = _interop.CreateGraphicsDevice(_d3dDevice);
			_compositionDevice2 = _compositionDevice.QueryInterface<ICompositionGraphicsDevice2>();
			_drawingSurface = _compositionDevice2.CreateDrawingSurface2(default(UnmanagedMethods.SIZE), DirectXPixelFormat.B8G8R8A8UIntNormalized, DirectXAlphaMode.Premultiplied);
			_surface = _drawingSurface.QueryInterface<ICompositionSurface>();
			_surfaceInterop = _drawingSurface.QueryInterface<ICompositionDrawingSurfaceInterop>();
		}
		catch
		{
			_surface?.Dispose();
			_surfaceInterop?.Dispose();
			_drawingSurface?.Dispose();
			_compositionDevice2?.Dispose();
			_compositionDevice?.Dispose();
			_interop?.Dispose();
			_compositor?.Dispose();
			_d3dDevice?.Dispose();
			throw;
		}
	}

	public void Dispose()
	{
		_surface.Dispose();
		_surfaceInterop.Dispose();
		_drawingSurface.Dispose();
		_compositionDevice2.Dispose();
		_compositionDevice.Dispose();
		_interop.Dispose();
		_compositor.Dispose();
		_d3dDevice.Dispose();
	}

	public unsafe IDirect3D11TextureRenderTargetRenderSession BeginDraw()
	{
		if (IsCorrupted)
		{
			throw new RenderTargetCorruptedException();
		}
		IDisposable disposable = _window.BeginTransaction();
		bool flag = false;
		try
		{
			PixelSize size = _window.WindowInfo.Size;
			double scaling = _window.WindowInfo.Scaling;
			_window.ResizeIfNeeded(size);
			_window.SetSurface(_surface);
			UnmanagedMethods.POINT pOINT;
			void* pObject = default(void*);
			try
			{
				if (_size != size)
				{
					_surfaceInterop.Resize(new UnmanagedMethods.POINT
					{
						X = size.Width,
						Y = size.Height
					});
					_size = size;
				}
				Guid iID_ID3D11Texture2D = IID_ID3D11Texture2D;
				pOINT = _surfaceInterop.BeginDraw(null, &iID_ID3D11Texture2D, &pObject);
			}
			catch (Exception innerException)
			{
				_lost = true;
				throw new RenderTargetCorruptedException(innerException);
			}
			flag = true;
			PixelPoint offset = new PixelPoint(pOINT.X, pOINT.Y);
			using IUnknown texture = MicroComRuntime.CreateProxyFor<IUnknown>(pObject, ownsHandle: true);
			Session result = new Session(_surfaceInterop, texture, disposable, _size, offset, scaling);
			disposable = null;
			return result;
		}
		finally
		{
			if (disposable != null)
			{
				if (flag)
				{
					_surfaceInterop.EndDraw();
				}
				disposable.Dispose();
			}
		}
	}
}
