using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Platform;
using Avalonia.Rendering.SceneGraph;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Media;

public abstract class DrawingContext : IDisposable
{
	public record struct PushedState(DrawingContext context) : IDisposable
	{
		private readonly DrawingContext _context = context;

		private readonly int _level = context._states.Count;

		public void Dispose()
		{
			if (context?._states != null)
			{
				if (context._states.Count != _level)
				{
					throw new InvalidOperationException("Wrong Push/Pop state order");
				}
				context._states.Pop().Dispose();
			}
		}

		[CompilerGenerated]
		private readonly bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	private readonly record struct RestoreState(DrawingContext context, PushedStateType type) : IDisposable
	{
		public enum PushedStateType
		{
			None,
			Transform,
			Opacity,
			Clip,
			GeometryClip,
			OpacityMask
		}

		private readonly DrawingContext _context = context;

		private readonly PushedStateType _type = type;

		public void Dispose()
		{
			if (type != 0)
			{
				if (context._states == null)
				{
					throw new ObjectDisposedException("DrawingContext");
				}
				if (type == PushedStateType.Transform)
				{
					context.PopTransformCore();
				}
				else if (type == PushedStateType.Clip)
				{
					context.PopClipCore();
				}
				else if (type == PushedStateType.Opacity)
				{
					context.PopOpacityCore();
				}
				else if (type == PushedStateType.GeometryClip)
				{
					context.PopGeometryClipCore();
				}
				else if (type == PushedStateType.OpacityMask)
				{
					context.PopOpacityMaskCore();
				}
			}
		}

		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	private Stack<RestoreState>? _states;

	private static ThreadSafeObjectPool<Stack<RestoreState>> StateStackPool { get; } = ThreadSafeObjectPool<Stack<RestoreState>>.Default;

	internal DrawingContext()
	{
	}

	public void Dispose()
	{
		if (_states != null)
		{
			while (_states.Count > 0)
			{
				_states.Pop().Dispose();
			}
			StateStackPool.ReturnAndSetNull(ref _states);
		}
		DisposeCore();
	}

	protected abstract void DisposeCore();

	public virtual void DrawImage(IImage source, Rect rect)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		DrawImage(source, new Rect(source.Size), rect);
	}

	public virtual void DrawImage(IImage source, Rect sourceRect, Rect destRect)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		source.Draw(this, sourceRect, destRect);
	}

	internal abstract void DrawBitmap(IRef<IBitmapImpl> source, double opacity, Rect sourceRect, Rect destRect);

	public void DrawLine(IPen pen, Point p1, Point p2)
	{
		if (PenIsVisible(pen))
		{
			DrawLineCore(pen, p1, p2);
		}
	}

	protected abstract void DrawLineCore(IPen pen, Point p1, Point p2);

	public void DrawGeometry(IBrush? brush, IPen? pen, Geometry geometry)
	{
		if ((brush != null || PenIsVisible(pen)) && geometry.PlatformImpl != null)
		{
			DrawGeometryCore(brush, pen, geometry.PlatformImpl);
		}
	}

	public void DrawGeometry(IBrush? brush, IPen? pen, IGeometryImpl geometry)
	{
		if (brush != null || PenIsVisible(pen))
		{
			DrawGeometryCore(brush, pen, geometry);
		}
	}

	protected abstract void DrawGeometryCore(IBrush? brush, IPen? pen, IGeometryImpl geometry);

	public void DrawRectangle(IBrush? brush, IPen? pen, Rect rect, double radiusX = 0.0, double radiusY = 0.0, BoxShadows boxShadows = default(BoxShadows))
	{
		if (brush != null || PenIsVisible(pen) || boxShadows.Count != 0)
		{
			if (!MathUtilities.IsZero(radiusX))
			{
				radiusX = Math.Min(radiusX, rect.Width / 2.0);
			}
			if (!MathUtilities.IsZero(radiusY))
			{
				radiusY = Math.Min(radiusY, rect.Height / 2.0);
			}
			DrawRectangleCore(brush, pen, new RoundedRect(rect, radiusX, radiusY), boxShadows);
		}
	}

	public void DrawRectangle(IBrush? brush, IPen? pen, RoundedRect rrect, BoxShadows boxShadows = default(BoxShadows))
	{
		if (brush != null || PenIsVisible(pen) || boxShadows.Count != 0)
		{
			DrawRectangleCore(brush, pen, rrect, boxShadows);
		}
	}

	protected abstract void DrawRectangleCore(IBrush? brush, IPen? pen, RoundedRect rrect, BoxShadows boxShadows = default(BoxShadows));

	public void DrawRectangle(IPen pen, Rect rect, float cornerRadius = 0f)
	{
		DrawRectangle(null, pen, rect, cornerRadius, cornerRadius);
	}

	public void FillRectangle(IBrush brush, Rect rect, float cornerRadius = 0f)
	{
		DrawRectangle(brush, null, rect, cornerRadius, cornerRadius);
	}

	public void DrawEllipse(IBrush? brush, IPen? pen, Point center, double radiusX, double radiusY)
	{
		if (brush != null || PenIsVisible(pen))
		{
			double x = center.X - radiusX;
			double y = center.Y - radiusY;
			double width = radiusX * 2.0;
			double height = radiusY * 2.0;
			DrawEllipseCore(brush, pen, new Rect(x, y, width, height));
		}
	}

	public void DrawEllipse(IBrush? brush, IPen? pen, Rect rect)
	{
		if (brush != null || PenIsVisible(pen))
		{
			DrawEllipseCore(brush, pen, rect);
		}
	}

	protected abstract void DrawEllipseCore(IBrush? brush, IPen? pen, Rect rect);

	public abstract void Custom(ICustomDrawOperation custom);

	public virtual void DrawText(FormattedText text, Point origin)
	{
		if (text == null)
		{
			throw new ArgumentNullException("text");
		}
		text.Draw(this, origin);
	}

	public abstract void DrawGlyphRun(IBrush? foreground, GlyphRun glyphRun);

	public PushedState PushClip(RoundedRect clip)
	{
		PushClipCore(clip);
		if (_states == null)
		{
			_states = StateStackPool.Get();
		}
		_states.Push(new RestoreState(this, RestoreState.PushedStateType.Clip));
		return new PushedState(this);
	}

	protected abstract void PushClipCore(RoundedRect rect);

	public PushedState PushClip(Rect clip)
	{
		PushClipCore(clip);
		if (_states == null)
		{
			_states = StateStackPool.Get();
		}
		_states.Push(new RestoreState(this, RestoreState.PushedStateType.Clip));
		return new PushedState(this);
	}

	protected abstract void PushClipCore(Rect rect);

	public PushedState PushGeometryClip(Geometry clip)
	{
		PushGeometryClipCore(clip);
		if (_states == null)
		{
			_states = StateStackPool.Get();
		}
		_states.Push(new RestoreState(this, RestoreState.PushedStateType.GeometryClip));
		return new PushedState(this);
	}

	protected abstract void PushGeometryClipCore(Geometry clip);

	public PushedState PushOpacity(double opacity)
	{
		PushOpacityCore(opacity);
		if (_states == null)
		{
			_states = StateStackPool.Get();
		}
		_states.Push(new RestoreState(this, RestoreState.PushedStateType.Opacity));
		return new PushedState(this);
	}

	protected abstract void PushOpacityCore(double opacity);

	public PushedState PushOpacityMask(IBrush mask, Rect bounds)
	{
		PushOpacityMaskCore(mask, bounds);
		if (_states == null)
		{
			_states = StateStackPool.Get();
		}
		_states.Push(new RestoreState(this, RestoreState.PushedStateType.OpacityMask));
		return new PushedState(this);
	}

	protected abstract void PushOpacityMaskCore(IBrush mask, Rect bounds);

	public PushedState PushTransform(Matrix matrix)
	{
		PushTransformCore(matrix);
		if (_states == null)
		{
			_states = StateStackPool.Get();
		}
		_states.Push(new RestoreState(this, RestoreState.PushedStateType.Transform));
		return new PushedState(this);
	}

	[Obsolete("Use PushTransform")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public PushedState PushPreTransform(Matrix matrix)
	{
		return PushTransform(matrix);
	}

	[Obsolete("Use PushTransform")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public PushedState PushPostTransform(Matrix matrix)
	{
		return PushTransform(matrix);
	}

	[Obsolete("Use PushTransform")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public PushedState PushTransformContainer()
	{
		return PushTransform(Matrix.Identity);
	}

	protected abstract void PushTransformCore(Matrix matrix);

	protected abstract void PopClipCore();

	protected abstract void PopGeometryClipCore();

	protected abstract void PopOpacityCore();

	protected abstract void PopOpacityMaskCore();

	protected abstract void PopTransformCore();

	private static bool PenIsVisible(IPen? pen)
	{
		if (pen?.Brush != null)
		{
			return pen.Thickness > 0.0;
		}
		return false;
	}
}
