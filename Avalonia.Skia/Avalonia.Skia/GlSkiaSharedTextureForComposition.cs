using System;
using Avalonia.OpenGL;
using Avalonia.Rendering.Composition;

namespace Avalonia.Skia;

internal class GlSkiaSharedTextureForComposition : ICompositionImportableOpenGlSharedTexture, ICompositionImportableSharedGpuContextImage, IDisposable
{
	private readonly object _lock = new object();

	public IGlContext Context { get; }

	public int TextureId { get; private set; }

	public int InternalFormat { get; }

	public PixelSize Size { get; }

	public GlSkiaSharedTextureForComposition(IGlContext context, int textureId, int internalFormat, PixelSize size)
	{
		Context = context;
		TextureId = textureId;
		InternalFormat = internalFormat;
		Size = size;
	}

	public void Dispose(IGlContext context)
	{
		lock (_lock)
		{
			if (TextureId == 0)
			{
				return;
			}
			try
			{
				using (context.EnsureCurrent())
				{
					context.GlInterface.DeleteTexture(TextureId);
				}
			}
			catch
			{
			}
			TextureId = 0;
		}
	}

	public void Dispose()
	{
		Dispose(Context);
	}
}
