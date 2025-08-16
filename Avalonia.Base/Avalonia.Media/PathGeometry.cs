using System;
using System.Runtime.CompilerServices;
using Avalonia.Collections;
using Avalonia.Metadata;
using Avalonia.Platform;
using Avalonia.Visuals.Platform;

namespace Avalonia.Media;

public class PathGeometry : StreamGeometry
{
	public static readonly DirectProperty<PathGeometry, PathFigures?> FiguresProperty;

	public static readonly StyledProperty<FillRule> FillRuleProperty;

	private PathFigures? _figures;

	private IDisposable? _figuresObserver;

	private IDisposable? _figuresPropertiesObserver;

	[Content]
	public PathFigures? Figures
	{
		get
		{
			return _figures;
		}
		set
		{
			SetAndRaise(FiguresProperty, ref _figures, value);
		}
	}

	public FillRule FillRule
	{
		get
		{
			return GetValue(FillRuleProperty);
		}
		set
		{
			SetValue(FillRuleProperty, value);
		}
	}

	static PathGeometry()
	{
		FiguresProperty = AvaloniaProperty.RegisterDirect("Figures", (PathGeometry g) => g.Figures, delegate(PathGeometry g, PathFigures? f)
		{
			g.Figures = f;
		});
		FillRuleProperty = AvaloniaProperty.Register<PathGeometry, FillRule>("FillRule", FillRule.EvenOdd);
		FiguresProperty.Changed.AddClassHandler(delegate(PathGeometry s, AvaloniaPropertyChangedEventArgs e)
		{
			s.OnFiguresChanged(e.NewValue as PathFigures);
		});
	}

	public PathGeometry()
	{
		_figures = new PathFigures();
	}

	public new static PathGeometry Parse(string pathData)
	{
		PathGeometry pathGeometry = new PathGeometry();
		using PathGeometryContext geometryContext = new PathGeometryContext(pathGeometry);
		using PathMarkupParser pathMarkupParser = new PathMarkupParser(geometryContext);
		pathMarkupParser.Parse(pathData);
		return pathGeometry;
	}

	private protected sealed override IGeometryImpl? CreateDefiningGeometry()
	{
		PathFigures figures = Figures;
		if (figures == null)
		{
			return null;
		}
		IStreamGeometryImpl streamGeometryImpl = AvaloniaLocator.Current.GetRequiredService<IPlatformRenderInterface>().CreateStreamGeometry();
		using StreamGeometryContext streamGeometryContext = new StreamGeometryContext(streamGeometryImpl.Open());
		streamGeometryContext.SetFillRule(FillRule);
		foreach (PathFigure item in figures)
		{
			item.ApplyTo(streamGeometryContext);
		}
		return streamGeometryImpl;
	}

	private void OnFiguresChanged(PathFigures? figures)
	{
		_figuresObserver?.Dispose();
		_figuresPropertiesObserver?.Dispose();
		_figuresObserver = figures?.ForEachItem(delegate(PathFigure s)
		{
			s.SegmentsInvalidated += InvalidateGeometryFromSegments;
			InvalidateGeometry();
		}, delegate(PathFigure s)
		{
			s.SegmentsInvalidated -= InvalidateGeometryFromSegments;
			InvalidateGeometry();
		}, base.InvalidateGeometry);
		_figuresPropertiesObserver = figures?.TrackItemPropertyChanged(delegate
		{
			InvalidateGeometry();
		});
	}

	private void InvalidateGeometryFromSegments(object? _, EventArgs __)
	{
		InvalidateGeometry();
	}

	public override string ToString()
	{
		string text = ((_figures != null) ? string.Join(" ", _figures) : string.Empty);
		return FormattableString.Invariant(FormattableStringFactory.Create("{0}{1}", (FillRule != 0) ? "F1 " : "", text));
	}
}
