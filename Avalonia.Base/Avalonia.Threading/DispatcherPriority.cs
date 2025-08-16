using System;
using System.ComponentModel;
using Avalonia.Metadata;

namespace Avalonia.Threading;

public readonly struct DispatcherPriority : IEquatable<DispatcherPriority>, IComparable<DispatcherPriority>
{
	public static readonly DispatcherPriority Default = new DispatcherPriority(0);

	internal static readonly DispatcherPriority MinimumForegroundPriority = Default;

	public static readonly DispatcherPriority Input = new DispatcherPriority((int)Default - 1);

	public static readonly DispatcherPriority Background = new DispatcherPriority((int)Input - 1);

	public static readonly DispatcherPriority ContextIdle = new DispatcherPriority((int)Background - 1);

	public static readonly DispatcherPriority ApplicationIdle = new DispatcherPriority((int)ContextIdle - 1);

	public static readonly DispatcherPriority SystemIdle = new DispatcherPriority((int)ApplicationIdle - 1);

	internal static readonly DispatcherPriority MinimumActiveValue = new DispatcherPriority(SystemIdle);

	public static readonly DispatcherPriority Inactive = new DispatcherPriority((int)MinimumActiveValue - 1);

	internal static readonly DispatcherPriority MinValue = new DispatcherPriority(Inactive);

	public static readonly DispatcherPriority Invalid = new DispatcherPriority((int)MinimumActiveValue - 2);

	public static readonly DispatcherPriority Loaded = new DispatcherPriority((int)Default + 1);

	[PrivateApi]
	public static readonly DispatcherPriority UiThreadRender = new DispatcherPriority((int)Loaded + 1);

	internal static readonly DispatcherPriority AfterRender = new DispatcherPriority((int)UiThreadRender + 1);

	public static readonly DispatcherPriority Render = new DispatcherPriority((int)AfterRender + 1);

	[PrivateApi]
	public static readonly DispatcherPriority BeforeRender = new DispatcherPriority((int)Render + 1);

	[PrivateApi]
	public static readonly DispatcherPriority AsyncRenderTargetResize = new DispatcherPriority((int)BeforeRender + 1);

	[Obsolete("WPF compatibility")]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public static readonly DispatcherPriority DataBind = new DispatcherPriority(Render);

	public static readonly DispatcherPriority Normal = new DispatcherPriority((int)DataBind + 1);

	public static readonly DispatcherPriority Send = new DispatcherPriority((int)Normal + 1);

	public static readonly DispatcherPriority MaxValue = Send;

	public int Value { get; }

	private DispatcherPriority(int value)
	{
		Value = value;
	}

	public static DispatcherPriority FromValue(int value)
	{
		if (value < MinValue.Value || value > MaxValue.Value)
		{
			throw new ArgumentOutOfRangeException("value");
		}
		return new DispatcherPriority(value);
	}

	public static implicit operator int(DispatcherPriority priority)
	{
		return priority.Value;
	}

	public static implicit operator DispatcherPriority(int value)
	{
		return FromValue(value);
	}

	public bool Equals(DispatcherPriority other)
	{
		return Value == other.Value;
	}

	public override bool Equals(object? obj)
	{
		if (obj is DispatcherPriority other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return Value.GetHashCode();
	}

	public static bool operator ==(DispatcherPriority left, DispatcherPriority right)
	{
		return left.Value == right.Value;
	}

	public static bool operator !=(DispatcherPriority left, DispatcherPriority right)
	{
		return left.Value != right.Value;
	}

	public static bool operator <(DispatcherPriority left, DispatcherPriority right)
	{
		return left.Value < right.Value;
	}

	public static bool operator >(DispatcherPriority left, DispatcherPriority right)
	{
		return left.Value > right.Value;
	}

	public static bool operator <=(DispatcherPriority left, DispatcherPriority right)
	{
		return left.Value <= right.Value;
	}

	public static bool operator >=(DispatcherPriority left, DispatcherPriority right)
	{
		return left.Value >= right.Value;
	}

	public int CompareTo(DispatcherPriority other)
	{
		return Value.CompareTo(other.Value);
	}

	public static void Validate(DispatcherPriority priority, string parameterName)
	{
		if (priority < Inactive || priority > MaxValue)
		{
			throw new ArgumentException("Invalid DispatcherPriority value", parameterName);
		}
	}

	public override string ToString()
	{
		if (this == Invalid)
		{
			return "Invalid";
		}
		if (this == Inactive)
		{
			return "Inactive";
		}
		if (this == SystemIdle)
		{
			return "SystemIdle";
		}
		if (this == ContextIdle)
		{
			return "ContextIdle";
		}
		if (this == ApplicationIdle)
		{
			return "ApplicationIdle";
		}
		if (this == Background)
		{
			return "Background";
		}
		if (this == Input)
		{
			return "Input";
		}
		if (this == Default)
		{
			return "Default";
		}
		if (this == Loaded)
		{
			return "Loaded";
		}
		if (this == UiThreadRender)
		{
			return "UiThreadRender";
		}
		if (this == AfterRender)
		{
			return "AfterRender";
		}
		if (this == Render)
		{
			return "Render";
		}
		if (this == BeforeRender)
		{
			return "BeforeRender";
		}
		if (this == AsyncRenderTargetResize)
		{
			return "AsyncRenderTargetResize";
		}
		if (this == DataBind)
		{
			return "DataBind";
		}
		if (this == Normal)
		{
			return "Normal";
		}
		if (this == Send)
		{
			return "Send";
		}
		return Value.ToString();
	}
}
