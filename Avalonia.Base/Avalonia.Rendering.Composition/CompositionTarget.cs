using System;
using Avalonia.Collections.Pooled;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Server;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition;

internal class CompositionTarget : CompositionObject
{
	private CompositionTargetChangedFields _changedFieldsOfCompositionTarget;

	private CompositionVisual? _root;

	private bool _isEnabled;

	private RendererDebugOverlays _debugOverlays;

	private LayoutPassTiming _lastLayoutPassTiming;

	private double _scaling;

	private Size _size;

	internal new ServerCompositionTarget Server { get; }

	public CompositionVisual? Root
	{
		get
		{
			return _root;
		}
		set
		{
			bool flag = false;
			if (_root != value)
			{
				OnRootChanging();
				flag = true;
				_root = value;
				_changedFieldsOfCompositionTarget |= CompositionTargetChangedFields.Root;
				RegisterForSerialization();
			}
			_root = value;
			if (flag)
			{
				OnRootChanged();
			}
		}
	}

	public bool IsEnabled
	{
		get
		{
			return _isEnabled;
		}
		set
		{
			bool flag = false;
			if (_isEnabled != value)
			{
				flag = true;
				_isEnabled = value;
				_changedFieldsOfCompositionTarget |= CompositionTargetChangedFields.IsEnabled;
				RegisterForSerialization();
			}
			_isEnabled = value;
		}
	}

	public RendererDebugOverlays DebugOverlays
	{
		get
		{
			return _debugOverlays;
		}
		set
		{
			bool flag = false;
			if (_debugOverlays != value)
			{
				flag = true;
				_debugOverlays = value;
				_changedFieldsOfCompositionTarget |= CompositionTargetChangedFields.DebugOverlays;
				RegisterForSerialization();
			}
			_debugOverlays = value;
		}
	}

	internal LayoutPassTiming LastLayoutPassTiming
	{
		get
		{
			return _lastLayoutPassTiming;
		}
		set
		{
			bool flag = false;
			if (_lastLayoutPassTiming != value)
			{
				flag = true;
				_lastLayoutPassTiming = value;
				_changedFieldsOfCompositionTarget |= CompositionTargetChangedFields.LastLayoutPassTiming;
				RegisterForSerialization();
			}
			_lastLayoutPassTiming = value;
		}
	}

	public double Scaling
	{
		get
		{
			return _scaling;
		}
		set
		{
			bool flag = false;
			if (_scaling != value)
			{
				flag = true;
				_scaling = value;
				_changedFieldsOfCompositionTarget |= CompositionTargetChangedFields.Scaling;
				RegisterForSerialization();
			}
			_scaling = value;
		}
	}

	public Size Size
	{
		get
		{
			return _size;
		}
		set
		{
			bool flag = false;
			if (_size != value)
			{
				flag = true;
				_size = value;
				_changedFieldsOfCompositionTarget |= CompositionTargetChangedFields.Size;
				RegisterForSerialization();
			}
			_size = value;
		}
	}

	public PooledList<CompositionVisual>? TryHitTest(Point point, CompositionVisual? root, Func<CompositionVisual, bool>? filter)
	{
		Server.Readback.NextRead();
		if (root == null)
		{
			root = Root;
		}
		if (root == null)
		{
			return null;
		}
		PooledList<CompositionVisual> result = new PooledList<CompositionVisual>();
		HitTestCore(root, point, result, filter);
		return result;
	}

	public Point? TryTransformToVisual(CompositionVisual visual, Point point)
	{
		if (visual.Root != this)
		{
			return null;
		}
		CompositionVisual compositionVisual = visual;
		Matrix identity = Matrix.Identity;
		while (compositionVisual != null)
		{
			if (!TryGetInvertedTransform(compositionVisual, out var matrix))
			{
				return null;
			}
			identity *= matrix;
			compositionVisual = compositionVisual.Parent;
		}
		return point * identity;
	}

	private static bool TryGetInvertedTransform(CompositionVisual visual, out Matrix matrix)
	{
		Matrix? matrix2 = visual.TryGetServerGlobalTransform();
		if (!matrix2.HasValue)
		{
			matrix = default(Matrix);
			return false;
		}
		return matrix2.Value.TryInvert(out matrix);
	}

	private static bool TryTransformTo(CompositionVisual visual, Point globalPoint, out Point v)
	{
		v = default(Point);
		if (TryGetInvertedTransform(visual, out var matrix))
		{
			v = globalPoint * matrix;
			return true;
		}
		return false;
	}

	private void HitTestCore(CompositionVisual visual, Point globalPoint, PooledList<CompositionVisual> result, Func<CompositionVisual, bool>? filter)
	{
		if (!visual.Visible || (filter != null && !filter(visual)) || !TryTransformTo(visual, globalPoint, out var v) || (visual.ClipToBounds && (v.X < 0.0 || v.Y < 0.0 || v.X > visual.Size.X || v.Y > visual.Size.Y)))
		{
			return;
		}
		IGeometryImpl? clip = visual.Clip;
		if (clip != null && !clip.FillContains(v))
		{
			return;
		}
		if (visual is CompositionContainerVisual compositionContainerVisual)
		{
			for (int num = compositionContainerVisual.Children.Count - 1; num >= 0; num--)
			{
				CompositionVisual visual2 = compositionContainerVisual.Children[num];
				HitTestCore(visual2, globalPoint, result, filter);
			}
		}
		if (visual.HitTest(v))
		{
			result.Add(visual);
		}
	}

	public void RequestRedraw()
	{
		RegisterForSerialization();
	}

	internal CompositionTarget(Compositor compositor, ServerCompositionTarget server)
		: base(compositor, server)
	{
		Server = server;
		InitializeDefaults();
	}

	private void OnRootChanged()
	{
		if (Root != null)
		{
			Root.Root = this;
		}
	}

	private void OnRootChanging()
	{
		if (Root != null)
		{
			Root.Root = null;
		}
	}

	private void InitializeDefaults()
	{
	}

	private protected override void SerializeChangesCore(BatchStreamWriter writer)
	{
		base.SerializeChangesCore(writer);
		writer.Write(_changedFieldsOfCompositionTarget);
		if ((_changedFieldsOfCompositionTarget & CompositionTargetChangedFields.Root) == CompositionTargetChangedFields.Root)
		{
			writer.WriteObject(_root?.Server);
		}
		if ((_changedFieldsOfCompositionTarget & CompositionTargetChangedFields.IsEnabled) == CompositionTargetChangedFields.IsEnabled)
		{
			writer.Write(_isEnabled);
		}
		if ((_changedFieldsOfCompositionTarget & CompositionTargetChangedFields.DebugOverlays) == CompositionTargetChangedFields.DebugOverlays)
		{
			writer.Write(_debugOverlays);
		}
		if ((_changedFieldsOfCompositionTarget & CompositionTargetChangedFields.LastLayoutPassTiming) == CompositionTargetChangedFields.LastLayoutPassTiming)
		{
			writer.Write(_lastLayoutPassTiming);
		}
		if ((_changedFieldsOfCompositionTarget & CompositionTargetChangedFields.Scaling) == CompositionTargetChangedFields.Scaling)
		{
			writer.Write(_scaling);
		}
		if ((_changedFieldsOfCompositionTarget & CompositionTargetChangedFields.Size) == CompositionTargetChangedFields.Size)
		{
			writer.Write(_size);
		}
		_changedFieldsOfCompositionTarget = (CompositionTargetChangedFields)0;
	}
}
