using System;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Logging;
using Avalonia.Reactive;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL.Controls;

internal class OpenGlControlBaseResources : IAsyncDisposable
{
	private int _depthBuffer;

	private PixelSize _depthBufferSize;

	private readonly CompositionOpenGlSwapchain _swapchain;

	public int Fbo { get; private set; }

	public CompositionDrawingSurface Surface { get; }

	public IGlContext Context { get; private set; }

	public static OpenGlControlBaseResources? TryCreate(CompositionDrawingSurface surface, ICompositionGpuInterop interop, IOpenGlTextureSharingRenderInterfaceContextFeature feature)
	{
		IGlContext glContext;
		try
		{
			glContext = feature.CreateSharedContext();
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to initialize OpenGL: unable to create additional OpenGL context: {exception}", propertyValue);
			return null;
		}
		if (glContext == null)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to initialize OpenGL: unable to create additional OpenGL context.");
			return null;
		}
		return new OpenGlControlBaseResources(glContext, surface, interop, feature, null);
	}

	public static OpenGlControlBaseResources? TryCreate(IGlContext context, CompositionDrawingSurface surface, ICompositionGpuInterop interop, IGlContextExternalObjectsFeature externalObjects)
	{
		if (!interop.SupportedImageHandleTypes.Contains("D3D11TextureGlobalSharedHandle") || !externalObjects.SupportedExportableExternalImageTypes.Contains("D3D11TextureGlobalSharedHandle"))
		{
			return null;
		}
		return new OpenGlControlBaseResources(context, surface, interop, null, externalObjects);
	}

	public OpenGlControlBaseResources(IGlContext context, CompositionDrawingSurface surface, ICompositionGpuInterop interop, IOpenGlTextureSharingRenderInterfaceContextFeature? feature, IGlContextExternalObjectsFeature? externalObjects)
	{
		Context = context;
		Surface = surface;
		using (context.MakeCurrent())
		{
			Fbo = context.GlInterface.GenFramebuffer();
		}
		_swapchain = ((feature != null) ? new CompositionOpenGlSwapchain(context, interop, Surface, feature) : new CompositionOpenGlSwapchain(context, interop, Surface, externalObjects));
	}

	private void UpdateDepthRenderbuffer(PixelSize size)
	{
		if (!(size == _depthBufferSize) || _depthBuffer == 0)
		{
			GlInterface glInterface = Context.GlInterface;
			glInterface.GetIntegerv(36007, out var rv);
			if (_depthBuffer != 0)
			{
				glInterface.DeleteRenderbuffer(_depthBuffer);
			}
			_depthBuffer = glInterface.GenRenderbuffer();
			glInterface.BindRenderbuffer(36161, _depthBuffer);
			glInterface.RenderbufferStorage(36161, (Context.Version.Type == GlProfileType.OpenGLES) ? 33189 : 6402, size.Width, size.Height);
			glInterface.FramebufferRenderbuffer(36160, 36096, 36161, _depthBuffer);
			glInterface.BindRenderbuffer(36161, rv);
			_depthBufferSize = size;
		}
	}

	public IDisposable BeginDraw(PixelSize size)
	{
		IDisposable restoreContext = Context.EnsureCurrent();
		IDisposable imagePresent = null;
		bool flag = false;
		try
		{
			GlInterface glInterface = Context.GlInterface;
			Context.GlInterface.BindFramebuffer(36160, Fbo);
			UpdateDepthRenderbuffer(size);
			imagePresent = _swapchain.BeginDraw(size, out IGlTexture texture);
			glInterface.FramebufferTexture2D(36160, 36064, 3553, texture.TextureId, 0);
			if (glInterface.CheckFramebufferStatus(36160) != 36053)
			{
				int error = glInterface.GetError();
				Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to configure OpenGL FBO: {code}", error);
				throw OpenGlException.GetFormattedException("Unable to configure OpenGL FBO", error);
			}
			flag = true;
			return Disposable.Create(delegate
			{
				try
				{
					Context.GlInterface.Flush();
					imagePresent.Dispose();
				}
				finally
				{
					restoreContext.Dispose();
				}
			});
		}
		finally
		{
			if (!flag)
			{
				imagePresent?.Dispose();
				restoreContext.Dispose();
			}
		}
	}

	public async ValueTask DisposeAsync()
	{
		IGlContext context = Context;
		if (context == null || context.IsLost)
		{
			return;
		}
		try
		{
			using (Context.MakeCurrent())
			{
				GlInterface glInterface = Context.GlInterface;
				if (Fbo != 0)
				{
					glInterface.DeleteFramebuffer(Fbo);
				}
				Fbo = 0;
				if (_depthBuffer != 0)
				{
					glInterface.DeleteRenderbuffer(_depthBuffer);
				}
				_depthBuffer = 0;
			}
		}
		catch
		{
		}
		Surface.Dispose();
		await _swapchain.DisposeAsync();
		Context = null;
	}
}
