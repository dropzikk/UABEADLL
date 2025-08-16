using System;
using Avalonia.OpenGL;
using Avalonia.Platform;
using SkiaSharp;

namespace Avalonia.Skia;

internal class FboSkiaSurface : ISkiaSurface, IDisposable
{
	private readonly GlSkiaGpu _gpu;

	private readonly GRContext _grContext;

	private readonly IGlContext _glContext;

	private readonly PixelSize _pixelSize;

	private int _fbo;

	private int _depthStencil;

	private int _texture;

	private SKSurface? _surface;

	private static readonly bool[] TrueFalse = new bool[2] { true, false };

	public SKSurface Surface => _surface ?? throw new ObjectDisposedException("FboSkiaSurface");

	public bool CanBlit { get; }

	public FboSkiaSurface(GlSkiaGpu gpu, GRContext grContext, IGlContext glContext, PixelSize pixelSize, GRSurfaceOrigin surfaceOrigin)
	{
		_gpu = gpu;
		_grContext = grContext;
		_glContext = glContext;
		_pixelSize = pixelSize;
		int internalFormat = ((glContext.Version.Type == GlProfileType.OpenGLES) ? 6408 : 32856);
		GlInterface glInterface = glContext.GlInterface;
		glInterface.GetIntegerv(36006, out var rv);
		glInterface.GetIntegerv(36007, out var rv2);
		glInterface.GetIntegerv(32873, out var rv3);
		_fbo = glInterface.GenFramebuffer();
		glInterface.BindFramebuffer(36160, _fbo);
		_texture = glInterface.GenTexture();
		glInterface.BindTexture(3553, _texture);
		glInterface.TexImage2D(3553, 0, internalFormat, pixelSize.Width, pixelSize.Height, 0, 6408, 5121, IntPtr.Zero);
		glInterface.TexParameteri(3553, 10240, 9728);
		glInterface.TexParameteri(3553, 10241, 9728);
		glInterface.FramebufferTexture2D(36160, 36064, 3553, _texture, 0);
		bool flag = false;
		bool[] trueFalse = TrueFalse;
		foreach (bool num in trueFalse)
		{
			_depthStencil = glInterface.GenRenderbuffer();
			glInterface.BindRenderbuffer(36161, _depthStencil);
			if (num)
			{
				glInterface.RenderbufferStorage(36161, 36168, pixelSize.Width, pixelSize.Height);
				glInterface.FramebufferRenderbuffer(36160, 36128, 36161, _depthStencil);
			}
			else
			{
				glInterface.RenderbufferStorage(36161, 35056, pixelSize.Width, pixelSize.Height);
				glInterface.FramebufferRenderbuffer(36160, 36096, 36161, _depthStencil);
				glInterface.FramebufferRenderbuffer(36160, 36128, 36161, _depthStencil);
			}
			if (glInterface.CheckFramebufferStatus(36160) == 36053)
			{
				flag = true;
				break;
			}
			glInterface.BindRenderbuffer(36161, rv2);
			glInterface.DeleteRenderbuffer(_depthStencil);
		}
		glInterface.BindRenderbuffer(36161, rv2);
		glInterface.BindTexture(3553, rv3);
		glInterface.BindFramebuffer(36160, rv);
		if (!flag)
		{
			glInterface.DeleteFramebuffer(_fbo);
			glInterface.DeleteTexture(_texture);
			throw new OpenGlException("Unable to create FBO with stencil");
		}
		GRBackendRenderTarget renderTarget = new GRBackendRenderTarget(pixelSize.Width, pixelSize.Height, 0, 8, new GRGlFramebufferInfo((uint)_fbo, SKColorType.Rgba8888.ToGlSizedFormat()));
		_surface = SKSurface.Create(_grContext, renderTarget, surfaceOrigin, SKColorType.Rgba8888, new SKSurfaceProperties(SKPixelGeometry.RgbHorizontal));
		CanBlit = glInterface.IsBlitFramebufferAvailable;
	}

	public void Dispose()
	{
		try
		{
			using (_glContext.EnsureCurrent())
			{
				_surface?.Dispose();
				_surface = null;
				GlInterface glInterface = _glContext.GlInterface;
				if (_fbo != 0)
				{
					glInterface.DeleteFramebuffer(_fbo);
					glInterface.DeleteTexture(_texture);
					glInterface.DeleteRenderbuffer(_depthStencil);
				}
			}
		}
		catch (PlatformGraphicsContextLostException)
		{
			if (_surface != null)
			{
				_gpu.AddPostDispose(_surface.Dispose);
			}
			_surface = null;
		}
		finally
		{
			_fbo = (_texture = (_depthStencil = 0));
		}
	}

	public void Blit(SKCanvas canvas)
	{
		canvas.Clear();
		canvas.Flush();
		GlInterface glInterface = _glContext.GlInterface;
		glInterface.GetIntegerv(36010, out var rv);
		glInterface.BindFramebuffer(36008, _fbo);
		glInterface.BlitFramebuffer(0, 0, _pixelSize.Width, _pixelSize.Height, 0, 0, _pixelSize.Width, _pixelSize.Height, 16384, 9729);
		glInterface.BindFramebuffer(36008, rv);
	}
}
