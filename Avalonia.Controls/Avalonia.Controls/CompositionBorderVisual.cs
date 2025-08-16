using System;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Controls;

internal class CompositionBorderVisual : CompositionDrawListVisual
{
	private class ServerBorderVisual : ServerCompositionDrawListVisual
	{
		private CornerRadius _cornerRadius;

		protected override bool HandlesClipToBounds => true;

		public ServerBorderVisual(ServerCompositor compositor, Visual v)
			: base(compositor, v)
		{
		}

		protected override void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
		{
			if (base.ClipToBounds)
			{
				Rect bounds = base.Root.SnapToDevicePixels(new Rect(new Size(base.Size.X, base.Size.Y)));
				if (_cornerRadius == default(CornerRadius))
				{
					canvas.PushClip(bounds);
				}
				else
				{
					canvas.PushClip(new RoundedRect(in bounds, in _cornerRadius));
				}
			}
			base.RenderCore(canvas, currentTransformedClip);
			if (base.ClipToBounds)
			{
				canvas.PopClip();
			}
		}

		protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
		{
			base.DeserializeChangesCore(reader, committedAt);
			if (reader.Read<bool>())
			{
				_cornerRadius = reader.Read<CornerRadius>();
			}
		}
	}

	private CornerRadius _cornerRadius;

	private bool _cornerRadiusChanged;

	public CornerRadius CornerRadius
	{
		get
		{
			return _cornerRadius;
		}
		set
		{
			if (_cornerRadius != value)
			{
				_cornerRadiusChanged = true;
				_cornerRadius = value;
				RegisterForSerialization();
			}
		}
	}

	public CompositionBorderVisual(Compositor compositor, Visual visual)
		: base(compositor, new ServerBorderVisual(compositor.Server, visual), visual)
	{
	}

	private protected override void SerializeChangesCore(BatchStreamWriter writer)
	{
		base.SerializeChangesCore(writer);
		writer.Write(_cornerRadiusChanged);
		if (_cornerRadiusChanged)
		{
			writer.Write(_cornerRadius);
		}
	}
}
