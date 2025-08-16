using System;
using Avalonia.Logging;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionCustomVisual : ServerCompositionContainerVisual, IServerClockItem
{
	private readonly CompositionCustomVisualHandler _handler;

	private bool _wantsNextAnimationFrameAfterTick;

	public override Rect OwnContentBounds => _handler.GetRenderBounds();

	internal ServerCompositionCustomVisual(ServerCompositor compositor, CompositionCustomVisualHandler handler)
		: base(compositor)
	{
		_handler = handler ?? throw new ArgumentNullException("handler");
		_handler.Attach(this);
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		int num = reader.Read<int>();
		for (int i = 0; i < num; i++)
		{
			try
			{
				_handler.OnMessage(reader.ReadObject());
			}
			catch (Exception propertyValue)
			{
				Logger.TryGet(LogEventLevel.Error, "Visual")?.Log(_handler, $"Exception in {_handler.GetType().Name}.{"OnMessage"} {{0}}", propertyValue);
			}
		}
	}

	public void OnTick()
	{
		_wantsNextAnimationFrameAfterTick = false;
		_handler.OnAnimationFrameUpdate();
		if (!_wantsNextAnimationFrameAfterTick)
		{
			base.Compositor.RemoveFromClock(this);
		}
	}

	protected override void OnAttachedToRoot(ServerCompositionTarget target)
	{
		if (_wantsNextAnimationFrameAfterTick)
		{
			base.Compositor.AddToClock(this);
		}
		base.OnAttachedToRoot(target);
	}

	protected override void OnDetachedFromRoot(ServerCompositionTarget target)
	{
		base.Compositor.RemoveFromClock(this);
		base.OnDetachedFromRoot(target);
	}

	internal void HandlerInvalidate()
	{
		ValuesInvalidated();
	}

	internal void HandlerRegisterForNextAnimationFrameUpdate()
	{
		_wantsNextAnimationFrameAfterTick = true;
		if (base.Root != null)
		{
			base.Compositor.AddToClock(this);
		}
	}

	protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		using ImmediateDrawingContext drawingContext = new ImmediateDrawingContext(canvas, ownsImpl: false);
		try
		{
			_handler.OnRender(drawingContext);
		}
		catch (Exception propertyValue)
		{
			Logger.TryGet(LogEventLevel.Error, "Visual")?.Log(_handler, $"Exception in {_handler.GetType().Name}.{"OnRender"} {{0}}", propertyValue);
		}
	}
}
