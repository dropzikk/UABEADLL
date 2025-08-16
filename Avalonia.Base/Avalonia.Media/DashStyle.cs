using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Avalonia.Animation;
using Avalonia.Collections;
using Avalonia.Media.Immutable;
using Avalonia.Reactive;

namespace Avalonia.Media;

public sealed class DashStyle : Animatable, IDashStyle
{
	public static readonly StyledProperty<AvaloniaList<double>?> DashesProperty;

	public static readonly StyledProperty<double> OffsetProperty;

	private static ImmutableDashStyle? s_dash;

	private static ImmutableDashStyle? s_dot;

	private static ImmutableDashStyle? s_dashDot;

	private static ImmutableDashStyle? s_dashDotDot;

	public static IDashStyle Dash => s_dash ?? (s_dash = new ImmutableDashStyle(new double[2] { 2.0, 2.0 }, 1.0));

	public static IDashStyle Dot => s_dot ?? (s_dot = new ImmutableDashStyle(new double[2] { 0.0, 2.0 }, 0.0));

	public static IDashStyle DashDot => s_dashDot ?? (s_dashDot = new ImmutableDashStyle(new double[4] { 2.0, 2.0, 0.0, 2.0 }, 1.0));

	public static IDashStyle DashDotDot => s_dashDotDot ?? (s_dashDotDot = new ImmutableDashStyle(new double[6] { 2.0, 2.0, 0.0, 2.0, 0.0, 2.0 }, 1.0));

	public AvaloniaList<double>? Dashes
	{
		get
		{
			return GetValue(DashesProperty);
		}
		set
		{
			SetValue(DashesProperty, value);
		}
	}

	public double Offset
	{
		get
		{
			return GetValue(OffsetProperty);
		}
		set
		{
			SetValue(OffsetProperty, value);
		}
	}

	IReadOnlyList<double>? IDashStyle.Dashes => Dashes;

	internal event EventHandler? Invalidated;

	public DashStyle()
	{
	}

	public DashStyle(IEnumerable<double>? dashes, double offset)
	{
		Dashes = (dashes as AvaloniaList<double>) ?? new AvaloniaList<double>(dashes ?? Array.Empty<double>());
		Offset = offset;
	}

	static DashStyle()
	{
		DashesProperty = AvaloniaProperty.Register<DashStyle, AvaloniaList<double>>("Dashes");
		OffsetProperty = AvaloniaProperty.Register<DashStyle, double>("Offset", 0.0);
		AnonymousObserver<AvaloniaPropertyChangedEventArgs> observer = new AnonymousObserver<AvaloniaPropertyChangedEventArgs>(delegate(AvaloniaPropertyChangedEventArgs e)
		{
			((DashStyle)e.Sender).Invalidated?.Invoke(e.Sender, EventArgs.Empty);
		});
		DashesProperty.Changed.Subscribe(observer);
		OffsetProperty.Changed.Subscribe(observer);
	}

	public ImmutableDashStyle ToImmutable()
	{
		return new ImmutableDashStyle(Dashes, Offset);
	}

	protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
	{
		base.OnPropertyChanged(change);
		if (change.Property == DashesProperty)
		{
			var (avaloniaList, avaloniaList2) = change.GetOldAndNewValue<AvaloniaList<double>>();
			if (avaloniaList != null)
			{
				avaloniaList.CollectionChanged -= DashesChanged;
			}
			if (avaloniaList2 != null)
			{
				avaloniaList2.CollectionChanged += DashesChanged;
			}
		}
	}

	private void DashesChanged(object? sender, NotifyCollectionChangedEventArgs e)
	{
		this.Invalidated?.Invoke(this, e);
	}
}
