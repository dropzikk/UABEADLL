using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Data;
using Avalonia.Reactive;

namespace Avalonia.Animation;

internal class AnimatorKeyFrame : AvaloniaObject
{
	public static readonly DirectProperty<AnimatorKeyFrame, object?> ValueProperty = AvaloniaProperty.RegisterDirect("Value", (AnimatorKeyFrame k) => k.Value, delegate(AnimatorKeyFrame k, object? v)
	{
		k.Value = v;
	});

	internal bool isNeutral;

	private object? _value;

	public Type? AnimatorType { get; }

	public Func<IAnimator>? AnimatorFactory { get; }

	public Cue Cue { get; }

	public KeySpline? KeySpline { get; }

	public AvaloniaProperty? Property { get; private set; }

	public object? Value
	{
		get
		{
			return _value;
		}
		set
		{
			SetAndRaise(ValueProperty, ref _value, value);
		}
	}

	public AnimatorKeyFrame()
	{
	}

	public AnimatorKeyFrame(Type? animatorType, Func<IAnimator>? animatorFactory, Cue cue)
	{
		AnimatorType = animatorType;
		AnimatorFactory = animatorFactory;
		Cue = cue;
		KeySpline = null;
	}

	public AnimatorKeyFrame(Type? animatorType, Func<IAnimator>? animatorFactory, Cue cue, KeySpline? keySpline)
	{
		AnimatorType = animatorType;
		AnimatorFactory = animatorFactory;
		Cue = cue;
		KeySpline = keySpline;
	}

	public IDisposable BindSetter(IAnimationSetter setter, Animatable targetControl)
	{
		Property = setter.Property;
		object value = setter.Value;
		if (value is IBinding binding)
		{
			return this.Bind(ValueProperty, binding, targetControl);
		}
		return this.Bind(ValueProperty, Observable.SingleValue(value).ToBinding(), targetControl);
	}

	[RequiresUnreferencedCode("Conversion methods are required for type conversion, including op_Implicit, op_Explicit, Parse and TypeConverter.")]
	public T GetTypedValue<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.All)] T>()
	{
		TypeConverter converter = TypeDescriptor.GetConverter(typeof(T));
		if (Value == null)
		{
			throw new ArgumentNullException("KeyFrame value can't be null.");
		}
		object value = Value;
		if (value is T)
		{
			return (T)value;
		}
		if (!converter.CanConvertTo(Value.GetType()))
		{
			throw new InvalidCastException("KeyFrame value doesnt match property type.");
		}
		return (T)converter.ConvertTo(Value, typeof(T));
	}
}
