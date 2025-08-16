using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using Avalonia.Media.Imaging;
using Avalonia.Media.Immutable;
using Avalonia.Platform;
using Avalonia.Threading;
using Avalonia.Utilities;

namespace Avalonia.Media;

public sealed class ImmediateDrawingContext : IDisposable, IOptionalFeatureProvider
{
	private readonly struct TransformContainer
	{
		public readonly Matrix LocalTransform;

		public readonly Matrix ContainerTransform;

		public TransformContainer(Matrix localTransform, Matrix containerTransform)
		{
			LocalTransform = localTransform;
			ContainerTransform = containerTransform;
		}
	}

	public readonly record struct PushedState : IDisposable
	{
		public enum PushedStateType
		{
			None,
			Matrix,
			Opacity,
			Clip,
			MatrixContainer,
			GeometryClip,
			OpacityMask
		}

		private readonly int _level;

		private readonly ImmediateDrawingContext _context;

		private readonly Matrix _matrix;

		private readonly PushedStateType _type;

		internal PushedState(ImmediateDrawingContext context, PushedStateType type, Matrix matrix = default(Matrix))
		{
			if (context._states == null)
			{
				throw new ObjectDisposedException("ImmediateDrawingContext");
			}
			_context = context;
			_type = type;
			_matrix = matrix;
			_level = ++context._currentLevel;
			context._states.Push(this);
		}

		public void Dispose()
		{
			if (_type != 0)
			{
				if (_context._states == null || _context._transformContainers == null)
				{
					throw new ObjectDisposedException("DrawingContext");
				}
				if (_context._currentLevel != _level)
				{
					throw new InvalidOperationException("Wrong Push/Pop state order");
				}
				_context._currentLevel--;
				_context._states.Pop();
				if (_type == PushedStateType.Matrix)
				{
					_context.CurrentTransform = _matrix;
				}
				else if (_type == PushedStateType.Clip)
				{
					_context.PlatformImpl.PopClip();
				}
				else if (_type == PushedStateType.Opacity)
				{
					_context.PlatformImpl.PopOpacity();
				}
				else if (_type == PushedStateType.GeometryClip)
				{
					_context.PlatformImpl.PopGeometryClip();
				}
				else if (_type == PushedStateType.OpacityMask)
				{
					_context.PlatformImpl.PopOpacityMask();
				}
				else if (_type == PushedStateType.MatrixContainer)
				{
					TransformContainer transformContainer = _context._transformContainers.Pop();
					_context._currentContainerTransform = transformContainer.ContainerTransform;
					_context.CurrentTransform = transformContainer.LocalTransform;
				}
			}
		}

		[CompilerGenerated]
		private bool PrintMembers(StringBuilder builder)
		{
			return false;
		}
	}

	private readonly bool _ownsImpl;

	private int _currentLevel;

	private Stack<PushedState>? _states = StateStackPool.Get();

	private Stack<TransformContainer>? _transformContainers = TransformStackPool.Get();

	private Matrix _currentTransform = Matrix.Identity;

	private Matrix _currentContainerTransform;

	private static ThreadSafeObjectPool<Stack<PushedState>> StateStackPool { get; } = ThreadSafeObjectPool<Stack<PushedState>>.Default;

	private static ThreadSafeObjectPool<Stack<TransformContainer>> TransformStackPool { get; } = ThreadSafeObjectPool<Stack<TransformContainer>>.Default;

	public IDrawingContextImpl PlatformImpl { get; }

	public Matrix CurrentTransform
	{
		get
		{
			return _currentTransform;
		}
		private set
		{
			_currentTransform = value;
			Matrix transform = _currentTransform * _currentContainerTransform;
			PlatformImpl.Transform = transform;
		}
	}

	internal ImmediateDrawingContext(IDrawingContextImpl impl, bool ownsImpl)
	{
		_ownsImpl = ownsImpl;
		PlatformImpl = impl;
		_currentContainerTransform = impl.Transform;
	}

	public void DrawBitmap(Bitmap source, Rect rect)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		DrawBitmap(source, new Rect(source.Size), rect);
	}

	public void DrawBitmap(Bitmap source, Rect sourceRect, Rect destRect)
	{
		if (source == null)
		{
			throw new ArgumentNullException("source");
		}
		PlatformImpl.DrawBitmap(source.PlatformImpl.Item, 1.0, sourceRect, destRect);
	}

	public void DrawLine(ImmutablePen pen, Point p1, Point p2)
	{
		if (PenIsVisible(pen))
		{
			PlatformImpl.DrawLine(pen, p1, p2);
		}
	}

	public void DrawRectangle(IImmutableBrush? brush, ImmutablePen? pen, Rect rect, double radiusX = 0.0, double radiusY = 0.0, BoxShadows boxShadows = default(BoxShadows))
	{
		if (brush != null || PenIsVisible(pen))
		{
			if (!MathUtilities.IsZero(radiusX))
			{
				radiusX = Math.Min(radiusX, rect.Width / 2.0);
			}
			if (!MathUtilities.IsZero(radiusY))
			{
				radiusY = Math.Min(radiusY, rect.Height / 2.0);
			}
			PlatformImpl.DrawRectangle(brush, pen, new RoundedRect(rect, radiusX, radiusY), boxShadows);
		}
	}

	public void DrawRectangle(ImmutablePen pen, Rect rect, float cornerRadius = 0f)
	{
		DrawRectangle(null, pen, rect, cornerRadius, cornerRadius);
	}

	public void DrawEllipse(IImmutableBrush? brush, ImmutablePen? pen, Point center, double radiusX, double radiusY)
	{
		if (brush != null || PenIsVisible(pen))
		{
			double x = center.X - radiusX;
			double y = center.Y - radiusY;
			double width = radiusX * 2.0;
			double height = radiusY * 2.0;
			PlatformImpl.DrawEllipse(brush, pen, new Rect(x, y, width, height));
		}
	}

	public void DrawGlyphRun(IImmutableBrush foreground, IImmutableGlyphRunReference glyphRun)
	{
		if (glyphRun == null)
		{
			throw new ArgumentNullException("glyphRun");
		}
		if (glyphRun.GlyphRun != null)
		{
			PlatformImpl.DrawGlyphRun(foreground, glyphRun.GlyphRun.Item);
		}
	}

	public void FillRectangle(IImmutableBrush brush, Rect rect, float cornerRadius = 0f)
	{
		DrawRectangle(brush, null, rect, cornerRadius, cornerRadius);
	}

	public PushedState PushClip(RoundedRect clip)
	{
		PlatformImpl.PushClip(clip);
		return new PushedState(this, PushedState.PushedStateType.Clip);
	}

	public PushedState PushClip(Rect clip)
	{
		PlatformImpl.PushClip(clip);
		return new PushedState(this, PushedState.PushedStateType.Clip);
	}

	public PushedState PushOpacity(double opacity, Rect bounds)
	{
		PlatformImpl.PushOpacity(opacity, bounds);
		return new PushedState(this, PushedState.PushedStateType.Opacity);
	}

	public PushedState PushOpacityMask(IImmutableBrush mask, Rect bounds)
	{
		PlatformImpl.PushOpacityMask(mask, bounds);
		return new PushedState(this, PushedState.PushedStateType.OpacityMask);
	}

	public PushedState PushPostTransform(Matrix matrix)
	{
		return PushSetTransform(CurrentTransform * matrix);
	}

	public PushedState PushPreTransform(Matrix matrix)
	{
		return PushSetTransform(matrix * CurrentTransform);
	}

	public PushedState PushSetTransform(Matrix matrix)
	{
		Matrix currentTransform = CurrentTransform;
		CurrentTransform = matrix;
		return new PushedState(this, PushedState.PushedStateType.Matrix, currentTransform);
	}

	public PushedState PushTransformContainer()
	{
		if (_transformContainers == null)
		{
			throw new ObjectDisposedException("DrawingContext");
		}
		_transformContainers.Push(new TransformContainer(CurrentTransform, _currentContainerTransform));
		_currentContainerTransform = CurrentTransform * _currentContainerTransform;
		_currentTransform = Matrix.Identity;
		return new PushedState(this, PushedState.PushedStateType.MatrixContainer);
	}

	public void Dispose()
	{
		if (_states == null || _transformContainers == null)
		{
			throw new ObjectDisposedException("DrawingContext");
		}
		while (_states.Count != 0)
		{
			_states.Peek().Dispose();
		}
		StateStackPool.ReturnAndSetNull(ref _states);
		if (_transformContainers.Count != 0)
		{
			throw new InvalidOperationException("Transform container stack is non-empty");
		}
		TransformStackPool.ReturnAndSetNull(ref _transformContainers);
		if (_ownsImpl)
		{
			PlatformImpl.Dispose();
		}
	}

	private static bool PenIsVisible(IPen? pen)
	{
		if (pen?.Brush != null)
		{
			return pen.Thickness > 0.0;
		}
		return false;
	}

	public object? TryGetFeature(Type type)
	{
		return PlatformImpl.GetFeature(type);
	}
}
