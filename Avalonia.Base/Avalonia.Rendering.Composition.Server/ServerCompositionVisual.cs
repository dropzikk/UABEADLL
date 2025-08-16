using System;
using System.Numerics;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.Rendering.Composition.Expressions;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal abstract class ServerCompositionVisual : ServerObject
{
	public record struct UpdateResult(Rect? Bounds, bool InvalidatedOld, bool InvalidatedNew)
	{
		public UpdateResult(Rect? Bounds, bool InvalidatedOld, bool InvalidatedNew)
		{
			this.Bounds = Bounds;
			this.InvalidatedOld = InvalidatedOld;
			this.InvalidatedNew = InvalidatedNew;
		}

		public UpdateResult()
			: this(null, InvalidatedOld: false, InvalidatedNew: false)
		{
		}
	}

	public struct ReadbackData
	{
		public Matrix Matrix;

		public ulong Revision;

		public long TargetId;

		public bool Visible;
	}

	private bool _isDirtyForUpdate;

	private Rect _oldOwnContentBounds;

	private bool _isBackface;

	private Rect? _transformedClipBounds;

	private Rect _combinedTransformedClipBounds;

	private ReadbackData _readback0;

	private ReadbackData _readback1;

	private ReadbackData _readback2;

	protected bool IsDirtyComposition;

	private bool _combinedTransformDirty;

	private bool _clipSizeDirty;

	private const CompositionVisualChangedFields CompositionFieldsMask = CompositionVisualChangedFields.Opacity | CompositionVisualChangedFields.OpacityAnimated | CompositionVisualChangedFields.Clip | CompositionVisualChangedFields.ClipToBounds | CompositionVisualChangedFields.ClipToBoundsAnimated | CompositionVisualChangedFields.Size | CompositionVisualChangedFields.SizeAnimated | CompositionVisualChangedFields.OpacityMaskBrush;

	private const CompositionVisualChangedFields CombinedTransformFieldsMask = CompositionVisualChangedFields.Offset | CompositionVisualChangedFields.OffsetAnimated | CompositionVisualChangedFields.Size | CompositionVisualChangedFields.SizeAnimated | CompositionVisualChangedFields.AnchorPoint | CompositionVisualChangedFields.AnchorPointAnimated | CompositionVisualChangedFields.CenterPoint | CompositionVisualChangedFields.CenterPointAnimated | CompositionVisualChangedFields.RotationAngle | CompositionVisualChangedFields.RotationAngleAnimated | CompositionVisualChangedFields.Orientation | CompositionVisualChangedFields.OrientationAnimated | CompositionVisualChangedFields.Scale | CompositionVisualChangedFields.ScaleAnimated | CompositionVisualChangedFields.TransformMatrix | CompositionVisualChangedFields.AdornedVisual;

	private const CompositionVisualChangedFields ClipSizeDirtyMask = CompositionVisualChangedFields.ClipToBounds | CompositionVisualChangedFields.ClipToBoundsAnimated | CompositionVisualChangedFields.Size | CompositionVisualChangedFields.SizeAnimated;

	private ServerCompositionTarget? _root;

	internal static CompositionProperty s_IdOfRootProperty = CompositionProperty.Register();

	private ServerCompositionVisual? _parent;

	internal static CompositionProperty s_IdOfParentProperty = CompositionProperty.Register();

	private bool _visible;

	internal static CompositionProperty s_IdOfVisibleProperty = CompositionProperty.Register();

	private float _opacity;

	internal static CompositionProperty s_IdOfOpacityProperty = CompositionProperty.Register();

	private IGeometryImpl? _clip;

	internal static CompositionProperty s_IdOfClipProperty = CompositionProperty.Register();

	private bool _clipToBounds;

	internal static CompositionProperty s_IdOfClipToBoundsProperty = CompositionProperty.Register();

	private Vector3D _offset;

	internal static CompositionProperty s_IdOfOffsetProperty = CompositionProperty.Register();

	private Vector _size;

	internal static CompositionProperty s_IdOfSizeProperty = CompositionProperty.Register();

	private Vector _anchorPoint;

	internal static CompositionProperty s_IdOfAnchorPointProperty = CompositionProperty.Register();

	private Vector3D _centerPoint;

	internal static CompositionProperty s_IdOfCenterPointProperty = CompositionProperty.Register();

	private float _rotationAngle;

	internal static CompositionProperty s_IdOfRotationAngleProperty = CompositionProperty.Register();

	private Quaternion _orientation;

	internal static CompositionProperty s_IdOfOrientationProperty = CompositionProperty.Register();

	private Vector3D _scale;

	internal static CompositionProperty s_IdOfScaleProperty = CompositionProperty.Register();

	private Matrix _transformMatrix;

	internal static CompositionProperty s_IdOfTransformMatrixProperty = CompositionProperty.Register();

	private ServerCompositionVisual? _adornedVisual;

	internal static CompositionProperty s_IdOfAdornedVisualProperty = CompositionProperty.Register();

	private bool _adornerIsClipped;

	internal static CompositionProperty s_IdOfAdornerIsClippedProperty = CompositionProperty.Register();

	private IImmutableBrush? _opacityMaskBrush;

	internal static CompositionProperty s_IdOfOpacityMaskBrushProperty = CompositionProperty.Register();

	private IImmutableEffect? _effect;

	internal static CompositionProperty s_IdOfEffectProperty = CompositionProperty.Register();

	private RenderOptions _renderOptions;

	internal static CompositionProperty s_IdOfRenderOptionsProperty = CompositionProperty.Register();

	protected virtual bool HandlesClipToBounds => false;

	public Matrix CombinedTransformMatrix { get; private set; } = Matrix.Identity;

	public Matrix GlobalTransformMatrix { get; private set; }

	public bool IsVisibleInFrame { get; set; }

	public bool IsHitTestVisibleInFrame { get; set; }

	public double EffectiveOpacity { get; set; }

	public Rect TransformedOwnContentBounds { get; set; }

	public virtual Rect OwnContentBounds => new Rect(0.0, 0.0, Size.X, Size.Y);

	public ServerCompositionTarget? Root
	{
		get
		{
			return GetValue(s_IdOfRootProperty, ref _root);
		}
		set
		{
			bool flag = false;
			if (_root != value)
			{
				OnRootChanging();
				flag = true;
			}
			SetValue(s_IdOfRootProperty, ref _root, value);
			if (flag)
			{
				OnRootChanged();
			}
		}
	}

	public ServerCompositionVisual? Parent
	{
		get
		{
			return GetValue(s_IdOfParentProperty, ref _parent);
		}
		set
		{
			bool flag = false;
			if (_parent != value)
			{
				flag = true;
			}
			SetValue(s_IdOfParentProperty, ref _parent, value);
		}
	}

	public bool Visible
	{
		get
		{
			return GetAnimatedValue(s_IdOfVisibleProperty, ref _visible);
		}
		set
		{
			SetAnimatedValue(s_IdOfVisibleProperty, out _visible, value);
		}
	}

	public float Opacity
	{
		get
		{
			return GetAnimatedValue(s_IdOfOpacityProperty, ref _opacity);
		}
		set
		{
			SetAnimatedValue(s_IdOfOpacityProperty, out _opacity, value);
		}
	}

	public IGeometryImpl? Clip
	{
		get
		{
			return GetValue(s_IdOfClipProperty, ref _clip);
		}
		set
		{
			bool flag = false;
			if (_clip != value)
			{
				flag = true;
			}
			SetValue(s_IdOfClipProperty, ref _clip, value);
		}
	}

	public bool ClipToBounds
	{
		get
		{
			return GetAnimatedValue(s_IdOfClipToBoundsProperty, ref _clipToBounds);
		}
		set
		{
			SetAnimatedValue(s_IdOfClipToBoundsProperty, out _clipToBounds, value);
		}
	}

	public Vector3D Offset
	{
		get
		{
			return GetAnimatedValue(s_IdOfOffsetProperty, ref _offset);
		}
		set
		{
			SetAnimatedValue(s_IdOfOffsetProperty, out _offset, value);
		}
	}

	public Vector Size
	{
		get
		{
			return GetAnimatedValue(s_IdOfSizeProperty, ref _size);
		}
		set
		{
			SetAnimatedValue(s_IdOfSizeProperty, out _size, value);
		}
	}

	public Vector AnchorPoint
	{
		get
		{
			return GetAnimatedValue(s_IdOfAnchorPointProperty, ref _anchorPoint);
		}
		set
		{
			SetAnimatedValue(s_IdOfAnchorPointProperty, out _anchorPoint, value);
		}
	}

	public Vector3D CenterPoint
	{
		get
		{
			return GetAnimatedValue(s_IdOfCenterPointProperty, ref _centerPoint);
		}
		set
		{
			SetAnimatedValue(s_IdOfCenterPointProperty, out _centerPoint, value);
		}
	}

	public float RotationAngle
	{
		get
		{
			return GetAnimatedValue(s_IdOfRotationAngleProperty, ref _rotationAngle);
		}
		set
		{
			SetAnimatedValue(s_IdOfRotationAngleProperty, out _rotationAngle, value);
		}
	}

	public Quaternion Orientation
	{
		get
		{
			return GetAnimatedValue(s_IdOfOrientationProperty, ref _orientation);
		}
		set
		{
			SetAnimatedValue(s_IdOfOrientationProperty, out _orientation, value);
		}
	}

	public Vector3D Scale
	{
		get
		{
			return GetAnimatedValue(s_IdOfScaleProperty, ref _scale);
		}
		set
		{
			SetAnimatedValue(s_IdOfScaleProperty, out _scale, value);
		}
	}

	public Matrix TransformMatrix
	{
		get
		{
			return GetAnimatedValue(s_IdOfTransformMatrixProperty, ref _transformMatrix);
		}
		set
		{
			SetAnimatedValue(s_IdOfTransformMatrixProperty, out _transformMatrix, value);
		}
	}

	public ServerCompositionVisual? AdornedVisual
	{
		get
		{
			return GetValue(s_IdOfAdornedVisualProperty, ref _adornedVisual);
		}
		set
		{
			bool flag = false;
			if (_adornedVisual != value)
			{
				flag = true;
			}
			SetValue(s_IdOfAdornedVisualProperty, ref _adornedVisual, value);
		}
	}

	public bool AdornerIsClipped
	{
		get
		{
			return GetValue(s_IdOfAdornerIsClippedProperty, ref _adornerIsClipped);
		}
		set
		{
			bool flag = false;
			if (_adornerIsClipped != value)
			{
				flag = true;
			}
			SetValue(s_IdOfAdornerIsClippedProperty, ref _adornerIsClipped, value);
		}
	}

	public IImmutableBrush? OpacityMaskBrush
	{
		get
		{
			return GetValue(s_IdOfOpacityMaskBrushProperty, ref _opacityMaskBrush);
		}
		set
		{
			bool flag = false;
			if (_opacityMaskBrush != value)
			{
				flag = true;
			}
			SetValue(s_IdOfOpacityMaskBrushProperty, ref _opacityMaskBrush, value);
		}
	}

	public IImmutableEffect? Effect
	{
		get
		{
			return GetValue(s_IdOfEffectProperty, ref _effect);
		}
		set
		{
			bool flag = false;
			if (_effect != value)
			{
				flag = true;
			}
			SetValue(s_IdOfEffectProperty, ref _effect, value);
		}
	}

	public RenderOptions RenderOptions
	{
		get
		{
			return GetValue(s_IdOfRenderOptionsProperty, ref _renderOptions);
		}
		set
		{
			bool flag = false;
			if (_renderOptions != value)
			{
				flag = true;
			}
			SetValue(s_IdOfRenderOptionsProperty, ref _renderOptions, value);
		}
	}

	protected virtual void RenderCore(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
	}

	public void Render(CompositorDrawingContextProxy canvas, Rect currentTransformedClip)
	{
		if (!Visible || !IsVisibleInFrame || Opacity == 0f)
		{
			return;
		}
		currentTransformedClip = currentTransformedClip.Intersect(_combinedTransformedClipBounds);
		if (currentTransformedClip.Width == 0.0 && currentTransformedClip.Height == 0.0)
		{
			return;
		}
		Root.RenderedVisuals++;
		Rect rect = new Rect(new Size(Size.X, Size.Y));
		if (AdornedVisual != null)
		{
			canvas.PostTransform = Matrix.Identity;
			canvas.Transform = Matrix.Identity;
			if (AdornerIsClipped)
			{
				canvas.PushClip(AdornedVisual._combinedTransformedClipBounds);
			}
		}
		Matrix postTransform = (canvas.PostTransform = GlobalTransformMatrix);
		canvas.Transform = Matrix.Identity;
		if (Effect != null)
		{
			canvas.PushEffect(Effect);
		}
		if (Opacity != 1f)
		{
			canvas.PushOpacity(Opacity, ClipToBounds ? new Rect?(rect) : ((Rect?)null));
		}
		if (ClipToBounds && !HandlesClipToBounds)
		{
			canvas.PushClip(Root.SnapToDevicePixels(rect));
		}
		if (Clip != null)
		{
			canvas.PushGeometryClip(Clip);
		}
		if (OpacityMaskBrush != null)
		{
			canvas.PushOpacityMask(OpacityMaskBrush, rect);
		}
		canvas.RenderOptions = RenderOptions;
		RenderCore(canvas, currentTransformedClip);
		canvas.PostTransform = postTransform;
		canvas.Transform = Matrix.Identity;
		if (OpacityMaskBrush != null)
		{
			canvas.PopOpacityMask();
		}
		if (Clip != null)
		{
			canvas.PopGeometryClip();
		}
		if (ClipToBounds && !HandlesClipToBounds)
		{
			canvas.PopClip();
		}
		if (AdornedVisual != null && AdornerIsClipped)
		{
			canvas.PopClip();
		}
		if (Opacity != 1f)
		{
			canvas.PopOpacity();
		}
		if (Effect != null)
		{
			canvas.PopEffect();
		}
	}

	public ref ReadbackData GetReadback(int idx)
	{
		return idx switch
		{
			0 => ref _readback0, 
			1 => ref _readback1, 
			_ => ref _readback2, 
		};
	}

	public virtual UpdateResult Update(ServerCompositionTarget root)
	{
		if (Parent == null && Root == null)
		{
			return default(UpdateResult);
		}
		bool isVisibleInFrame = IsVisibleInFrame;
		if (Parent != null)
		{
			RenderOptions = RenderOptions.MergeWith(Parent.RenderOptions);
		}
		if (_combinedTransformDirty)
		{
			CombinedTransformMatrix = MatrixUtils.ComputeTransform(Size, AnchorPoint, CenterPoint, (AdornedVisual != null) ? Matrix.Identity : TransformMatrix, Scale, RotationAngle, Orientation, Offset);
			_combinedTransformDirty = false;
		}
		Matrix matrix = (AdornedVisual ?? Parent)?.GlobalTransformMatrix ?? Matrix.Identity;
		Matrix matrix2 = CombinedTransformMatrix * matrix;
		bool flag = false;
		if (GlobalTransformMatrix != matrix2)
		{
			_isBackface = Vector3.Transform(new Vector3(0f, 0f, float.PositiveInfinity), MatrixUtils.ToMatrix4x4(GlobalTransformMatrix)).Z <= 0f;
			flag = true;
		}
		Rect transformedOwnContentBounds = TransformedOwnContentBounds;
		Rect combinedTransformedClipBounds = _combinedTransformedClipBounds;
		ServerCompositionVisual? parent = _parent;
		if (parent != null && parent.IsDirtyComposition)
		{
			IsDirtyComposition = true;
			_isDirtyForUpdate = true;
		}
		bool flag2 = _isDirtyForUpdate;
		bool flag3 = _isDirtyForUpdate;
		GlobalTransformMatrix = matrix2;
		Rect rect = OwnContentBounds;
		if (Effect != null)
		{
			rect = rect.Inflate(Effect.GetEffectOutputPadding());
		}
		if (rect != _oldOwnContentBounds || flag)
		{
			_oldOwnContentBounds = rect;
			if (rect.Width == 0.0 && rect.Height == 0.0)
			{
				TransformedOwnContentBounds = default(Rect);
			}
			else
			{
				TransformedOwnContentBounds = rect.TransformToAABB(GlobalTransformMatrix);
			}
		}
		if (_clipSizeDirty || flag)
		{
			_transformedClipBounds = (ClipToBounds ? new Rect?(new Rect(new Size(Size.X, Size.Y)).TransformToAABB(GlobalTransformMatrix)) : ((Rect?)null));
			_clipSizeDirty = false;
		}
		_combinedTransformedClipBounds = AdornedVisual?._combinedTransformedClipBounds ?? ((Parent?.Effect != null) ? ((Rect?)null) : Parent?._combinedTransformedClipBounds) ?? new Rect(Root.Size);
		if (_transformedClipBounds.HasValue)
		{
			_combinedTransformedClipBounds = _combinedTransformedClipBounds.Intersect(_transformedClipBounds.Value);
		}
		EffectiveOpacity = (double)Opacity * (Parent?.EffectiveOpacity ?? 1.0);
		ServerCompositionVisual? parent2 = _parent;
		IsHitTestVisibleInFrame = (parent2 == null || parent2.IsHitTestVisibleInFrame) && Visible && !_isBackface && (_combinedTransformedClipBounds.Width != 0.0 || _combinedTransformedClipBounds.Height != 0.0);
		int isVisibleInFrame2;
		if (IsHitTestVisibleInFrame)
		{
			ServerCompositionVisual? parent3 = _parent;
			if (parent3 == null || parent3.IsVisibleInFrame)
			{
				isVisibleInFrame2 = ((EffectiveOpacity > 0.04) ? 1 : 0);
				goto IL_03ae;
			}
		}
		isVisibleInFrame2 = 0;
		goto IL_03ae;
		IL_03ae:
		IsVisibleInFrame = (byte)isVisibleInFrame2 != 0;
		if (isVisibleInFrame != IsVisibleInFrame || flag)
		{
			flag2 = flag2 || isVisibleInFrame;
			flag3 |= IsVisibleInFrame;
		}
		if (flag3)
		{
			AddDirtyRect(TransformedOwnContentBounds.Intersect(_combinedTransformedClipBounds));
		}
		if (flag2)
		{
			AddDirtyRect(transformedOwnContentBounds.Intersect(combinedTransformedClipBounds));
		}
		_isDirtyForUpdate = false;
		ReadbackIndices readback = Root.Readback;
		ref ReadbackData readback2 = ref GetReadback(readback.WriteIndex);
		readback2.Revision = root.Revision;
		readback2.Matrix = GlobalTransformMatrix;
		readback2.TargetId = Root.Id;
		readback2.Visible = IsHitTestVisibleInFrame;
		return new UpdateResult(TransformedOwnContentBounds, flag3, flag2);
	}

	protected void AddDirtyRect(Rect rc)
	{
		if (!(rc == default(Rect)))
		{
			Root?.AddDirtyRect(rc);
		}
	}

	protected virtual void OnDetachedFromRoot(ServerCompositionTarget target)
	{
	}

	protected virtual void OnAttachedToRoot(ServerCompositionTarget target)
	{
	}

	protected override void ValuesInvalidated()
	{
		_isDirtyForUpdate = true;
		Root?.Invalidate();
	}

	public override void NotifyAnimatedValueChanged(CompositionProperty offset)
	{
		base.NotifyAnimatedValueChanged(offset);
		if (offset == s_IdOfClipToBoundsProperty || offset == s_IdOfOpacityProperty || offset == s_IdOfSizeProperty)
		{
			IsDirtyComposition = true;
		}
		if (offset == s_IdOfSizeProperty || offset == s_IdOfAnchorPointProperty || offset == s_IdOfCenterPointProperty || offset == s_IdOfAdornedVisualProperty || offset == s_IdOfTransformMatrixProperty || offset == s_IdOfScaleProperty || offset == s_IdOfRotationAngleProperty || offset == s_IdOfOrientationProperty || offset == s_IdOfOffsetProperty)
		{
			_combinedTransformDirty = true;
		}
		if (offset == s_IdOfClipToBoundsProperty || offset == s_IdOfSizeProperty)
		{
			_clipSizeDirty = true;
		}
	}

	internal ServerCompositionVisual(ServerCompositor compositor)
		: base(compositor)
	{
	}

	private void DeserializeChangesExtra(BatchStreamReader c)
	{
		ValuesInvalidated();
	}

	private void OnRootChanged()
	{
		if (Root != null)
		{
			Root.AddVisual(this);
			OnAttachedToRoot(Root);
		}
	}

	private void OnRootChanging()
	{
		if (Root != null)
		{
			Root.RemoveVisual(this);
			OnDetachedFromRoot(Root);
		}
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		DeserializeChangesExtra(reader);
		CompositionVisualChangedFields compositionVisualChangedFields = reader.Read<CompositionVisualChangedFields>();
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.Root) == CompositionVisualChangedFields.Root)
		{
			Root = reader.ReadObject<ServerCompositionTarget>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.Parent) == CompositionVisualChangedFields.Parent)
		{
			Parent = reader.ReadObject<ServerCompositionVisual>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.VisibleAnimated) == CompositionVisualChangedFields.VisibleAnimated)
		{
			SetAnimatedValue(s_IdOfVisibleProperty, ref _visible, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.Visible) == CompositionVisualChangedFields.Visible)
		{
			Visible = reader.Read<bool>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.OpacityAnimated) == CompositionVisualChangedFields.OpacityAnimated)
		{
			SetAnimatedValue(s_IdOfOpacityProperty, ref _opacity, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.Opacity) == CompositionVisualChangedFields.Opacity)
		{
			Opacity = reader.Read<float>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.Clip) == CompositionVisualChangedFields.Clip)
		{
			Clip = reader.ReadObject<IGeometryImpl>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.ClipToBoundsAnimated) == CompositionVisualChangedFields.ClipToBoundsAnimated)
		{
			SetAnimatedValue(s_IdOfClipToBoundsProperty, ref _clipToBounds, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.ClipToBounds) == CompositionVisualChangedFields.ClipToBounds)
		{
			ClipToBounds = reader.Read<bool>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.OffsetAnimated) == CompositionVisualChangedFields.OffsetAnimated)
		{
			SetAnimatedValue(s_IdOfOffsetProperty, ref _offset, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.Offset) == CompositionVisualChangedFields.Offset)
		{
			Offset = reader.Read<Vector3D>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.SizeAnimated) == CompositionVisualChangedFields.SizeAnimated)
		{
			SetAnimatedValue(s_IdOfSizeProperty, ref _size, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.Size) == CompositionVisualChangedFields.Size)
		{
			Size = reader.Read<Vector>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.AnchorPointAnimated) == CompositionVisualChangedFields.AnchorPointAnimated)
		{
			SetAnimatedValue(s_IdOfAnchorPointProperty, ref _anchorPoint, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.AnchorPoint) == CompositionVisualChangedFields.AnchorPoint)
		{
			AnchorPoint = reader.Read<Vector>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.CenterPointAnimated) == CompositionVisualChangedFields.CenterPointAnimated)
		{
			SetAnimatedValue(s_IdOfCenterPointProperty, ref _centerPoint, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.CenterPoint) == CompositionVisualChangedFields.CenterPoint)
		{
			CenterPoint = reader.Read<Vector3D>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.RotationAngleAnimated) == CompositionVisualChangedFields.RotationAngleAnimated)
		{
			SetAnimatedValue(s_IdOfRotationAngleProperty, ref _rotationAngle, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.RotationAngle) == CompositionVisualChangedFields.RotationAngle)
		{
			RotationAngle = reader.Read<float>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.OrientationAnimated) == CompositionVisualChangedFields.OrientationAnimated)
		{
			SetAnimatedValue(s_IdOfOrientationProperty, ref _orientation, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.Orientation) == CompositionVisualChangedFields.Orientation)
		{
			Orientation = reader.Read<Quaternion>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.ScaleAnimated) == CompositionVisualChangedFields.ScaleAnimated)
		{
			SetAnimatedValue(s_IdOfScaleProperty, ref _scale, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.Scale) == CompositionVisualChangedFields.Scale)
		{
			Scale = reader.Read<Vector3D>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.TransformMatrixAnimated) == CompositionVisualChangedFields.TransformMatrixAnimated)
		{
			SetAnimatedValue(s_IdOfTransformMatrixProperty, ref _transformMatrix, committedAt, reader.ReadObject<IAnimationInstance>());
		}
		else if ((compositionVisualChangedFields & CompositionVisualChangedFields.TransformMatrix) == CompositionVisualChangedFields.TransformMatrix)
		{
			TransformMatrix = reader.Read<Matrix>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.AdornedVisual) == CompositionVisualChangedFields.AdornedVisual)
		{
			AdornedVisual = reader.ReadObject<ServerCompositionVisual>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.AdornerIsClipped) == CompositionVisualChangedFields.AdornerIsClipped)
		{
			AdornerIsClipped = reader.Read<bool>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.OpacityMaskBrush) == CompositionVisualChangedFields.OpacityMaskBrush)
		{
			OpacityMaskBrush = reader.ReadObject<IImmutableBrush>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.Effect) == CompositionVisualChangedFields.Effect)
		{
			Effect = reader.ReadObject<IImmutableEffect>();
		}
		if ((compositionVisualChangedFields & CompositionVisualChangedFields.RenderOptions) == CompositionVisualChangedFields.RenderOptions)
		{
			RenderOptions = reader.Read<RenderOptions>();
		}
		OnFieldsDeserialized(compositionVisualChangedFields);
	}

	private void OnFieldsDeserialized(CompositionVisualChangedFields changed)
	{
		if ((changed & (CompositionVisualChangedFields.Opacity | CompositionVisualChangedFields.OpacityAnimated | CompositionVisualChangedFields.Clip | CompositionVisualChangedFields.ClipToBounds | CompositionVisualChangedFields.ClipToBoundsAnimated | CompositionVisualChangedFields.Size | CompositionVisualChangedFields.SizeAnimated | CompositionVisualChangedFields.OpacityMaskBrush)) != 0)
		{
			IsDirtyComposition = true;
		}
		if ((changed & (CompositionVisualChangedFields.Offset | CompositionVisualChangedFields.OffsetAnimated | CompositionVisualChangedFields.Size | CompositionVisualChangedFields.SizeAnimated | CompositionVisualChangedFields.AnchorPoint | CompositionVisualChangedFields.AnchorPointAnimated | CompositionVisualChangedFields.CenterPoint | CompositionVisualChangedFields.CenterPointAnimated | CompositionVisualChangedFields.RotationAngle | CompositionVisualChangedFields.RotationAngleAnimated | CompositionVisualChangedFields.Orientation | CompositionVisualChangedFields.OrientationAnimated | CompositionVisualChangedFields.Scale | CompositionVisualChangedFields.ScaleAnimated | CompositionVisualChangedFields.TransformMatrix | CompositionVisualChangedFields.AdornedVisual)) != 0)
		{
			_combinedTransformDirty = true;
		}
		if ((changed & (CompositionVisualChangedFields.ClipToBounds | CompositionVisualChangedFields.ClipToBoundsAnimated | CompositionVisualChangedFields.Size | CompositionVisualChangedFields.SizeAnimated)) != 0)
		{
			_clipSizeDirty = true;
		}
	}

	public override ExpressionVariant GetPropertyForAnimation(string name)
	{
		return name switch
		{
			"Visible" => Visible, 
			"Opacity" => Opacity, 
			"ClipToBounds" => ClipToBounds, 
			"RotationAngle" => RotationAngle, 
			"Orientation" => Orientation, 
			"AdornerIsClipped" => AdornerIsClipped, 
			_ => base.GetPropertyForAnimation(name), 
		};
	}

	public override CompositionProperty? GetCompositionProperty(string name)
	{
		return name switch
		{
			"Visible" => s_IdOfVisibleProperty, 
			"Opacity" => s_IdOfOpacityProperty, 
			"ClipToBounds" => s_IdOfClipToBoundsProperty, 
			"RotationAngle" => s_IdOfRotationAngleProperty, 
			"Orientation" => s_IdOfOrientationProperty, 
			"AdornerIsClipped" => s_IdOfAdornerIsClippedProperty, 
			_ => base.GetCompositionProperty(name), 
		};
	}
}
