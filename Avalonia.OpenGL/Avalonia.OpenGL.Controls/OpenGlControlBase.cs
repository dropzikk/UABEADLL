using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Logging;
using Avalonia.Platform;
using Avalonia.Rendering;
using Avalonia.Rendering.Composition;
using Avalonia.VisualTree;

namespace Avalonia.OpenGL.Controls;

public abstract class OpenGlControlBase : Control
{
	private CompositionSurfaceVisual? _visual;

	private readonly Action _update;

	private bool _updateQueued;

	private Task<bool>? _initialization;

	private OpenGlControlBaseResources? _resources;

	private Compositor? _compositor;

	protected GlVersion GlVersion => _resources?.Context.Version ?? default(GlVersion);

	public OpenGlControlBase()
	{
		_update = Update;
	}

	private void DoCleanup()
	{
		Task<bool> initialization = _initialization;
		if (initialization != null && initialization.Status == TaskStatus.RanToCompletion && _resources != null)
		{
			try
			{
				using (_resources.Context.EnsureCurrent())
				{
					OnOpenGlDeinit(_resources.Context.GlInterface);
				}
			}
			catch (Exception propertyValue)
			{
				Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to free user OpenGL resources: {exception}", propertyValue);
			}
		}
		ElementComposition.SetElementChildVisual(this, null);
		_visual = null;
		_resources?.DisposeAsync();
		_resources = null;
		_initialization = null;
	}

	protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
	{
		DoCleanup();
		base.OnDetachedFromVisualTree(e);
	}

	protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
	{
		base.OnAttachedToVisualTree(e);
		_compositor = (this.GetVisualRoot()?.Renderer as IRendererWithCompositor)?.Compositor;
		RequestNextFrameRendering();
	}

	[MemberNotNullWhen(true, "_resources")]
	private bool EnsureInitializedCore(ICompositionGpuInterop interop, IOpenGlTextureSharingRenderInterfaceContextFeature? contextSharingFeature)
	{
		CompositionDrawingSurface surface = _compositor.CreateDrawingSurface();
		IGlContext glContext = null;
		try
		{
			if (contextSharingFeature != null && contextSharingFeature.CanCreateSharedContext)
			{
				_resources = OpenGlControlBaseResources.TryCreate(surface, interop, contextSharingFeature);
			}
			if (_resources == null)
			{
				glContext = AvaloniaLocator.Current.GetRequiredService<IPlatformGraphicsOpenGlContextFactory>().CreateContext(null);
				if (glContext.TryGetFeature<IGlContextExternalObjectsFeature>(out var rv))
				{
					_resources = OpenGlControlBaseResources.TryCreate(glContext, surface, interop, rv);
				}
				else
				{
					glContext.Dispose();
				}
			}
			if (_resources == null)
			{
				Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to initialize OpenGL: current platform does not support multithreaded context sharing and shared memory");
				return false;
			}
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to initialize OpenGL: {exception}", propertyValue);
			glContext?.Dispose();
			return false;
		}
		_visual = _compositor.CreateSurfaceVisual();
		_visual.Size = new Vector(base.Bounds.Width, base.Bounds.Height);
		_visual.Surface = _resources.Surface;
		ElementComposition.SetElementChildVisual(this, _visual);
		using (_resources.Context.MakeCurrent())
		{
			OnOpenGlInit(_resources.Context.GlInterface);
		}
		return true;
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		if (_visual != null && change.Property == Visual.BoundsProperty)
		{
			_visual.Size = new Vector(base.Bounds.Width, base.Bounds.Height);
			RequestNextFrameRendering();
		}
		base.OnPropertyChanged(change);
	}

	private void ContextLost()
	{
		_initialization = null;
		_resources?.DisposeAsync();
		OnOpenGlLost();
	}

	[MemberNotNullWhen(true, "_resources")]
	private bool EnsureInitialized()
	{
		if (_initialization != null)
		{
			Task<bool> initialization = _initialization;
			if (initialization == null || !initialization.IsCompleted || initialization.Result)
			{
				Task<bool>? initialization2 = _initialization;
				if (initialization2 == null || !initialization2.IsFaulted)
				{
					initialization = _initialization;
					if (initialization != null && !initialization.IsCompleted)
					{
						return false;
					}
					if (_resources.Context.IsLost)
					{
						ContextLost();
						goto IL_0068;
					}
					return true;
				}
			}
			return false;
		}
		goto IL_0068;
		IL_0068:
		_initialization = InitializeAsync();
		ContinueOnInitialization();
		return false;
		async void ContinueOnInitialization()
		{
			try
			{
				await _initialization;
				RequestNextFrameRendering();
			}
			catch
			{
			}
		}
	}

	private void Update()
	{
		_updateQueued = false;
		IRenderRoot visualRoot = base.VisualRoot;
		if (visualRoot == null || !EnsureInitialized())
		{
			return;
		}
		using (_resources.BeginDraw(GetPixelSize(visualRoot)))
		{
			OnOpenGlRender(_resources.Context.GlInterface, _resources.Fbo);
		}
	}

	private async Task<bool> InitializeAsync()
	{
		if (_compositor == null)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Unable to obtain Compositor instance");
			return false;
		}
		ValueTask<ICompositionGpuInterop?> gpuInteropTask = _compositor.TryGetCompositionGpuInterop();
		IOpenGlTextureSharingRenderInterfaceContextFeature contextSharingFeature = (IOpenGlTextureSharingRenderInterfaceContextFeature)(await _compositor.TryGetRenderInterfaceFeature(typeof(IOpenGlTextureSharingRenderInterfaceContextFeature)));
		ICompositionGpuInterop compositionGpuInterop = await gpuInteropTask;
		if (compositionGpuInterop == null)
		{
			Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("OpenGlControlBase", "Compositor backend doesn't support GPU interop");
			return false;
		}
		if (!EnsureInitializedCore(compositionGpuInterop, contextSharingFeature))
		{
			DoCleanup();
			return false;
		}
		using (_resources.Context.MakeCurrent())
		{
			OnOpenGlInit(_resources.Context.GlInterface);
		}
		return true;
	}

	[Obsolete("Use RequestNextFrameRendering()")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public new void InvalidateVisual()
	{
		RequestNextFrameRendering();
	}

	public void RequestNextFrameRendering()
	{
		if (_initialization != null)
		{
			Task<bool> initialization = _initialization;
			if (initialization == null || initialization.Status != TaskStatus.RanToCompletion)
			{
				return;
			}
		}
		if (!_updateQueued)
		{
			_updateQueued = true;
			_compositor?.RequestCompositionUpdate(_update);
		}
	}

	private PixelSize GetPixelSize(IRenderRoot visualRoot)
	{
		double renderScaling = visualRoot.RenderScaling;
		return new PixelSize(Math.Max(1, (int)(base.Bounds.Width * renderScaling)), Math.Max(1, (int)(base.Bounds.Height * renderScaling)));
	}

	protected virtual void OnOpenGlInit(GlInterface gl)
	{
	}

	protected virtual void OnOpenGlDeinit(GlInterface gl)
	{
	}

	protected virtual void OnOpenGlLost()
	{
	}

	protected abstract void OnOpenGlRender(GlInterface gl, int fb);
}
