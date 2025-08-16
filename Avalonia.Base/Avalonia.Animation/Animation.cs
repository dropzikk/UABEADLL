using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Animation.Animators;
using Avalonia.Animation.Easings;
using Avalonia.Data;
using Avalonia.Media;
using Avalonia.Metadata;
using Avalonia.Reactive;

namespace Avalonia.Animation;

public sealed class Animation : AvaloniaObject, IAnimation
{
	private static readonly List<(Func<AvaloniaProperty, bool> Condition, Type Animator, Func<IAnimator> Factory)> Animators;

	public static readonly DirectProperty<Animation, TimeSpan> DurationProperty;

	public static readonly DirectProperty<Animation, IterationCount> IterationCountProperty;

	public static readonly DirectProperty<Animation, PlaybackDirection> PlaybackDirectionProperty;

	public static readonly DirectProperty<Animation, FillMode> FillModeProperty;

	public static readonly DirectProperty<Animation, Easing> EasingProperty;

	public static readonly DirectProperty<Animation, TimeSpan> DelayProperty;

	public static readonly DirectProperty<Animation, TimeSpan> DelayBetweenIterationsProperty;

	public static readonly DirectProperty<Animation, double> SpeedRatioProperty;

	private TimeSpan _duration;

	private IterationCount _iterationCount = new IterationCount(1uL);

	private PlaybackDirection _playbackDirection;

	private FillMode _fillMode;

	private Easing _easing = new LinearEasing();

	private TimeSpan _delay = TimeSpan.Zero;

	private TimeSpan _delayBetweenIterations = TimeSpan.Zero;

	private double _speedRatio = 1.0;

	private static readonly Dictionary<IAnimationSetter, (Type Type, Func<IAnimator> Factory)> s_animators;

	public TimeSpan Duration
	{
		get
		{
			return _duration;
		}
		set
		{
			SetAndRaise(DurationProperty, ref _duration, value);
		}
	}

	public IterationCount IterationCount
	{
		get
		{
			return _iterationCount;
		}
		set
		{
			SetAndRaise(IterationCountProperty, ref _iterationCount, value);
		}
	}

	public PlaybackDirection PlaybackDirection
	{
		get
		{
			return _playbackDirection;
		}
		set
		{
			SetAndRaise(PlaybackDirectionProperty, ref _playbackDirection, value);
		}
	}

	public FillMode FillMode
	{
		get
		{
			return _fillMode;
		}
		set
		{
			SetAndRaise(FillModeProperty, ref _fillMode, value);
		}
	}

	public Easing Easing
	{
		get
		{
			return _easing;
		}
		set
		{
			SetAndRaise(EasingProperty, ref _easing, value);
		}
	}

	public TimeSpan Delay
	{
		get
		{
			return _delay;
		}
		set
		{
			SetAndRaise(DelayProperty, ref _delay, value);
		}
	}

	public TimeSpan DelayBetweenIterations
	{
		get
		{
			return _delayBetweenIterations;
		}
		set
		{
			SetAndRaise(DelayBetweenIterationsProperty, ref _delayBetweenIterations, value);
		}
	}

	public double SpeedRatio
	{
		get
		{
			return _speedRatio;
		}
		set
		{
			SetAndRaise(SpeedRatioProperty, ref _speedRatio, value);
		}
	}

	[Content]
	public KeyFrames Children { get; } = new KeyFrames();

	[Obsolete("CustomAnimatorBase will be removed before 11.0, use InterpolatingAnimator<T>", true)]
	public static void SetAnimator(IAnimationSetter setter, CustomAnimatorBase value)
	{
		s_animators[setter] = (value.WrapperType, value.CreateWrapper);
	}

	public static void SetAnimator(IAnimationSetter setter, ICustomAnimator value)
	{
		s_animators[setter] = (value.WrapperType, value.CreateWrapper);
	}

	static Animation()
	{
		Animators = new List<(Func<AvaloniaProperty, bool>, Type, Func<IAnimator>)>
		{
			((AvaloniaProperty prop) => typeof(double).IsAssignableFrom(prop.PropertyType) && typeof(Transform).IsAssignableFrom(prop.OwnerType), typeof(TransformAnimator), () => new TransformAnimator()),
			((AvaloniaProperty prop) => typeof(bool).IsAssignableFrom(prop.PropertyType), typeof(BoolAnimator), () => new BoolAnimator()),
			((AvaloniaProperty prop) => typeof(byte).IsAssignableFrom(prop.PropertyType), typeof(ByteAnimator), () => new ByteAnimator()),
			((AvaloniaProperty prop) => typeof(short).IsAssignableFrom(prop.PropertyType), typeof(Int16Animator), () => new Int16Animator()),
			((AvaloniaProperty prop) => typeof(int).IsAssignableFrom(prop.PropertyType), typeof(Int32Animator), () => new Int32Animator()),
			((AvaloniaProperty prop) => typeof(long).IsAssignableFrom(prop.PropertyType), typeof(Int64Animator), () => new Int64Animator()),
			((AvaloniaProperty prop) => typeof(ushort).IsAssignableFrom(prop.PropertyType), typeof(UInt16Animator), () => new UInt16Animator()),
			((AvaloniaProperty prop) => typeof(uint).IsAssignableFrom(prop.PropertyType), typeof(UInt32Animator), () => new UInt32Animator()),
			((AvaloniaProperty prop) => typeof(ulong).IsAssignableFrom(prop.PropertyType), typeof(UInt64Animator), () => new UInt64Animator()),
			((AvaloniaProperty prop) => typeof(float).IsAssignableFrom(prop.PropertyType), typeof(FloatAnimator), () => new FloatAnimator()),
			((AvaloniaProperty prop) => typeof(double).IsAssignableFrom(prop.PropertyType), typeof(DoubleAnimator), () => new DoubleAnimator()),
			((AvaloniaProperty prop) => typeof(decimal).IsAssignableFrom(prop.PropertyType), typeof(DecimalAnimator), () => new DecimalAnimator())
		};
		DurationProperty = AvaloniaProperty.RegisterDirect("Duration", (Animation o) => o._duration, delegate(Animation o, TimeSpan v)
		{
			o._duration = v;
		});
		IterationCountProperty = AvaloniaProperty.RegisterDirect("IterationCount", (Animation o) => o._iterationCount, delegate(Animation o, IterationCount v)
		{
			o._iterationCount = v;
		});
		PlaybackDirectionProperty = AvaloniaProperty.RegisterDirect("PlaybackDirection", (Animation o) => o._playbackDirection, delegate(Animation o, PlaybackDirection v)
		{
			o._playbackDirection = v;
		}, PlaybackDirection.Normal);
		FillModeProperty = AvaloniaProperty.RegisterDirect("FillMode", (Animation o) => o._fillMode, delegate(Animation o, FillMode v)
		{
			o._fillMode = v;
		}, FillMode.None);
		EasingProperty = AvaloniaProperty.RegisterDirect("Easing", (Animation o) => o._easing, delegate(Animation o, Easing v)
		{
			o._easing = v;
		});
		DelayProperty = AvaloniaProperty.RegisterDirect("Delay", (Animation o) => o._delay, delegate(Animation o, TimeSpan v)
		{
			o._delay = v;
		});
		DelayBetweenIterationsProperty = AvaloniaProperty.RegisterDirect("DelayBetweenIterations", (Animation o) => o._delayBetweenIterations, delegate(Animation o, TimeSpan v)
		{
			o._delayBetweenIterations = v;
		});
		SpeedRatioProperty = AvaloniaProperty.RegisterDirect("SpeedRatio", (Animation o) => o._speedRatio, delegate(Animation o, double v)
		{
			o._speedRatio = v;
		}, 0.0, BindingMode.TwoWay);
		s_animators = new Dictionary<IAnimationSetter, (Type, Func<IAnimator>)>();
		RegisterAnimator<IEffect, EffectAnimator>();
		RegisterAnimator<BoxShadow, BoxShadowAnimator>();
		RegisterAnimator<BoxShadows, BoxShadowsAnimator>();
		RegisterAnimator<IBrush, BaseBrushAnimator>();
		RegisterAnimator<CornerRadius, CornerRadiusAnimator>();
		RegisterAnimator<Color, ColorAnimator>();
		RegisterAnimator<Vector, VectorAnimator>();
		RegisterAnimator<Point, PointAnimator>();
		RegisterAnimator<Rect, RectAnimator>();
		RegisterAnimator<RelativePoint, RelativePointAnimator>();
		RegisterAnimator<Size, SizeAnimator>();
		RegisterAnimator<Thickness, ThicknessAnimator>();
	}

	private static void RegisterAnimator<T, TAnimator>() where TAnimator : Animator<T>, new()
	{
		Animators.Insert(0, ((AvaloniaProperty prop) => typeof(T).IsAssignableFrom(prop.PropertyType), typeof(TAnimator), () => new TAnimator()));
	}

	public static void RegisterCustomAnimator<T, TAnimator>() where TAnimator : InterpolatingAnimator<T>, new()
	{
		Animators.Insert(0, ((AvaloniaProperty prop) => typeof(T).IsAssignableFrom(prop.PropertyType), typeof(InterpolatingAnimator<T>.AnimatorWrapper), () => new TAnimator().CreateWrapper()));
	}

	private static (Type Type, Func<IAnimator> Factory)? GetAnimatorType(AvaloniaProperty property)
	{
		foreach (var (func, item, item2) in Animators)
		{
			if (func(property))
			{
				return (item, item2);
			}
		}
		return null;
	}

	internal static (Type Type, Func<IAnimator> Factory)? GetAnimator(IAnimationSetter setter)
	{
		if (s_animators.TryGetValue(setter, out (Type, Func<IAnimator>) value))
		{
			return value;
		}
		return null;
	}

	private (IList<IAnimator> Animators, IList<IDisposable> subscriptions) InterpretKeyframes(Animatable control)
	{
		Dictionary<(Type, AvaloniaProperty), Func<IAnimator>> dictionary = new Dictionary<(Type, AvaloniaProperty), Func<IAnimator>>();
		List<AnimatorKeyFrame> list = new List<AnimatorKeyFrame>();
		List<IDisposable> list2 = new List<IDisposable>();
		foreach (KeyFrame child in Children)
		{
			foreach (IAnimationSetter setter in child.Setters)
			{
				if ((object)setter.Property == null)
				{
					throw new InvalidOperationException("No Setter property assigned.");
				}
				(Type, Func<IAnimator>)? tuple = GetAnimator(setter) ?? GetAnimatorType(setter.Property);
				if (!tuple.HasValue)
				{
					throw new InvalidOperationException($"No animator registered for the property {setter.Property}. Add an animator to the Animation.Animators collection that matches this property to animate it.");
				}
				var (type, func) = tuple.Value;
				if (!dictionary.ContainsKey((type, setter.Property)))
				{
					dictionary[(type, setter.Property)] = func;
				}
				Cue cue = child.Cue;
				if (child.TimingMode == KeyFrameTimingMode.TimeSpan)
				{
					cue = new Cue(child.KeyTime.TotalSeconds / Duration.TotalSeconds);
				}
				AnimatorKeyFrame animatorKeyFrame = new AnimatorKeyFrame(type, func, cue, child.KeySpline);
				list2.Add(animatorKeyFrame.BindSetter(setter, control));
				list.Add(animatorKeyFrame);
			}
		}
		List<IAnimator> list3 = new List<IAnimator>();
		foreach (KeyValuePair<(Type, AvaloniaProperty), Func<IAnimator>> item in dictionary)
		{
			IAnimator animator = item.Value();
			animator.Property = item.Key.Item2;
			list3.Add(animator);
		}
		foreach (AnimatorKeyFrame keyframe in list)
		{
			list3.First((IAnimator a) => a.GetType() == keyframe.AnimatorType && a.Property == keyframe.Property).Add(keyframe);
		}
		return (Animators: list3, subscriptions: list2);
	}

	IDisposable IAnimation.Apply(Animatable control, IClock? clock, IObservable<bool> match, Action? onComplete)
	{
		return Apply(control, clock, match, onComplete);
	}

	internal IDisposable Apply(Animatable control, IClock? clock, IObservable<bool> match, Action? onComplete)
	{
		var (list, list2) = InterpretKeyframes(control);
		if (list.Count == 1)
		{
			IDisposable disposable = list[0].Apply(this, control, clock, match, onComplete);
			if (disposable != null)
			{
				list2.Add(disposable);
			}
		}
		else
		{
			List<Task> list3 = ((onComplete != null) ? new List<Task>() : null);
			foreach (IAnimator item in list)
			{
				Action onComplete2 = null;
				if (onComplete != null)
				{
					TaskCompletionSource<object?> tcs = new TaskCompletionSource<object>();
					onComplete2 = delegate
					{
						tcs.SetResult(null);
					};
					list3.Add(tcs.Task);
				}
				IDisposable disposable2 = item.Apply(this, control, clock, match, onComplete2);
				if (disposable2 != null)
				{
					list2.Add(disposable2);
				}
			}
			if (onComplete != null)
			{
				Task.WhenAll(list3).ContinueWith(delegate(Task _, object? state)
				{
					((Action)state)();
				}, onComplete, TaskScheduler.FromCurrentSynchronizationContext());
			}
		}
		return new CompositeDisposable(list2);
	}

	public Task RunAsync(Animatable control, CancellationToken cancellationToken = default(CancellationToken))
	{
		return RunAsync(control, null, cancellationToken);
	}

	internal Task RunAsync(Animatable control, IClock? clock)
	{
		return RunAsync(control, clock, default(CancellationToken));
	}

	Task IAnimation.RunAsync(Animatable control, IClock? clock, CancellationToken cancellationToken)
	{
		return RunAsync(control, clock, cancellationToken);
	}

	internal Task RunAsync(Animatable control, IClock? clock, CancellationToken cancellationToken)
	{
		if (cancellationToken.IsCancellationRequested)
		{
			return Task.CompletedTask;
		}
		TaskCompletionSource<object?> run = new TaskCompletionSource<object>();
		if (IterationCount == IterationCount.Infinite)
		{
			run.SetException(new InvalidOperationException("Looping animations must not use the Run method."));
		}
		IDisposable subscriptions = null;
		IDisposable cancellation = null;
		subscriptions = Apply(control, clock, Observable.Return(value: true), delegate
		{
			run.TrySetResult(null);
			subscriptions?.Dispose();
			cancellation?.Dispose();
		});
		cancellation = cancellationToken.Register(delegate
		{
			run.TrySetResult(null);
			subscriptions?.Dispose();
			cancellation?.Dispose();
		});
		return run.Task;
	}
}
