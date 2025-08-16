using System;
using System.Runtime.InteropServices;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.OpenGL.Surfaces;
using Avalonia.Win32.DirectX;

namespace Avalonia.Win32.OpenGl.Angle;

internal class AngleD3DTextureFeature : IGlPlatformSurfaceRenderTargetFactory
{
	private class RenderTargetWrapper : EglPlatformSurfaceRenderTargetBase
	{
		private readonly AngleWin32EglDisplay _angle;

		private readonly IDirect3D11TextureRenderTarget _target;

		public override bool IsCorrupted
		{
			get
			{
				if (!_target.IsCorrupted)
				{
					return base.IsCorrupted;
				}
				return true;
			}
		}

		public RenderTargetWrapper(EglContext context, AngleWin32EglDisplay angle, IDirect3D11TextureRenderTarget target)
			: base(context)
		{
			_angle = angle;
			_target = target;
		}

		public override IGlPlatformSurfaceRenderingSession BeginDrawCore()
		{
			bool flag = false;
			IDisposable contextLock = base.Context.EnsureCurrent();
			IDirect3D11TextureRenderTargetRenderSession session = null;
			EglSurface surface = null;
			try
			{
				try
				{
					session = _target.BeginDraw();
				}
				catch (RenderTargetCorruptedException ex)
				{
					if (ex.InnerException is COMException ex2 && ((DXGI_ERROR)ex2.HResult).IsDeviceLostError())
					{
						base.Context.NotifyContextLost();
					}
					throw;
				}
				surface = _angle.WrapDirect3D11Texture(session.D3D11Texture2D, session.Offset.X, session.Offset.Y, session.Size.Width, session.Size.Height);
				IGlPlatformSurfaceRenderingSession result = BeginDraw(surface, session.Size, session.Scaling, delegate
				{
					using (contextLock)
					{
						using (session)
						{
							using (surface)
							{
							}
						}
					}
				}, isYFlipped: true);
				flag = true;
				return result;
			}
			finally
			{
				if (!flag)
				{
					using (contextLock)
					{
						using (session)
						{
							using (surface)
							{
							}
						}
					}
				}
			}
		}

		public override void Dispose()
		{
			_target.Dispose();
			base.Dispose();
		}
	}

	public bool CanRenderToSurface(IGlContext context, object surface)
	{
		if (context is EglContext { Display: AngleWin32EglDisplay { PlatformApi: AngleOptions.PlatformApi.DirectX11 } })
		{
			return surface is IDirect3D11TexturePlatformSurface;
		}
		return false;
	}

	public IGlPlatformSurfaceRenderTarget CreateRenderTarget(IGlContext context, object surface)
	{
		EglContext eglContext = (EglContext)context;
		AngleWin32EglDisplay angleWin32EglDisplay = (AngleWin32EglDisplay)eglContext.Display;
		IDirect3D11TexturePlatformSurface direct3D11TexturePlatformSurface = (IDirect3D11TexturePlatformSurface)surface;
		try
		{
			IDirect3D11TextureRenderTarget target = direct3D11TexturePlatformSurface.CreateRenderTarget(context, angleWin32EglDisplay.GetDirect3DDevice());
			return new RenderTargetWrapper(eglContext, angleWin32EglDisplay, target);
		}
		catch (COMException ex)
		{
			if (((DXGI_ERROR)ex.HResult).IsDeviceLostError())
			{
				eglContext.NotifyContextLost();
			}
			throw;
		}
	}
}
