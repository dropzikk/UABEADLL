using System;
using Avalonia.OpenGL;
using Avalonia.OpenGL.Egl;
using Avalonia.Platform;
using Avalonia.Win32.DirectX;
using MicroCom.Runtime;

namespace Avalonia.Win32.OpenGl.Angle;

internal class AngleExternalMemoryD3D11Texture2D : IGlExternalImageTexture, IDisposable
{
	private readonly EglContext _context;

	private ID3D11Texture2D? _texture2D;

	private EglSurface? _eglSurface;

	private IDXGIKeyedMutex? _mutex;

	private IDXGIKeyedMutex Mutex => _mutex ?? throw new ObjectDisposedException("AngleExternalMemoryD3D11Texture2D");

	public int TextureId { get; private set; }

	public int InternalFormat { get; }

	public PlatformGraphicsExternalImageProperties Properties { get; }

	public unsafe AngleExternalMemoryD3D11Texture2D(EglContext context, ID3D11Texture2D texture2D, PlatformGraphicsExternalImageProperties props)
	{
		_context = context;
		_texture2D = texture2D.CloneReference();
		_mutex = _texture2D.QueryInterface<IDXGIKeyedMutex>();
		Properties = props;
		InternalFormat = 32856;
		EglDisplay display = _context.Display;
		IntPtr nativeIntPtr = texture2D.GetNativeIntPtr();
		int[] obj = new int[13]
		{
			12375, 0, 12374, 0, 12416, 12382, 12417, 12383, 13405, 6408,
			12344, 12344, 12344
		};
		obj[1] = props.Width;
		obj[3] = props.Height;
		_eglSurface = display.CreatePBufferFromClientBuffer(13219, nativeIntPtr, obj);
		GlInterface glInterface = _context.GlInterface;
		int textureId = 0;
		glInterface.GenTextures(1, &textureId);
		TextureId = textureId;
		glInterface.BindTexture(3553, TextureId);
		if (_context.Display.EglInterface.BindTexImage(_context.Display.Handle, _eglSurface.DangerousGetHandle(), 12420) == 0)
		{
			throw OpenGlException.GetFormattedException("eglBindTexImage", _context.Display.EglInterface);
		}
	}

	public void Dispose()
	{
		if (!_context.IsLost && TextureId != 0)
		{
			using (_context.EnsureCurrent())
			{
				_context.GlInterface.DeleteTexture(TextureId);
			}
		}
		TextureId = 0;
		_eglSurface?.Dispose();
		_eglSurface = null;
		_texture2D?.Dispose();
		_texture2D = null;
		_mutex?.Dispose();
		_mutex = null;
	}

	public void AcquireKeyedMutex(uint key)
	{
		Mutex.AcquireSync(key, 2147483647u);
	}

	public void ReleaseKeyedMutex(uint key)
	{
		Mutex.ReleaseSync(key);
	}
}
