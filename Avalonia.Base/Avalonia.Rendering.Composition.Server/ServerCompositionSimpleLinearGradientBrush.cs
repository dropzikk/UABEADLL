using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleLinearGradientBrush : ServerCompositionSimpleGradientBrush, ILinearGradientBrush, IGradientBrush, IBrush
{
	private RelativePoint _startPoint;

	internal static CompositionProperty s_IdOfStartPointProperty = CompositionProperty.Register();

	private RelativePoint _endPoint;

	internal static CompositionProperty s_IdOfEndPointProperty = CompositionProperty.Register();

	public RelativePoint StartPoint
	{
		get
		{
			return GetValue(s_IdOfStartPointProperty, ref _startPoint);
		}
		set
		{
			bool flag = false;
			if (_startPoint != value)
			{
				flag = true;
			}
			SetValue(s_IdOfStartPointProperty, ref _startPoint, value);
		}
	}

	public RelativePoint EndPoint
	{
		get
		{
			return GetValue(s_IdOfEndPointProperty, ref _endPoint);
		}
		set
		{
			bool flag = false;
			if (_endPoint != value)
			{
				flag = true;
			}
			SetValue(s_IdOfEndPointProperty, ref _endPoint, value);
		}
	}

	internal ServerCompositionSimpleLinearGradientBrush(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionSimpleLinearGradientBrushChangedFields num = reader.Read<CompositionSimpleLinearGradientBrushChangedFields>();
		if ((num & CompositionSimpleLinearGradientBrushChangedFields.StartPoint) == CompositionSimpleLinearGradientBrushChangedFields.StartPoint)
		{
			StartPoint = reader.Read<RelativePoint>();
		}
		if ((num & CompositionSimpleLinearGradientBrushChangedFields.EndPoint) == CompositionSimpleLinearGradientBrushChangedFields.EndPoint)
		{
			EndPoint = reader.Read<RelativePoint>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, RelativePoint startPoint, RelativePoint endPoint)
	{
		writer.Write(CompositionSimpleLinearGradientBrushChangedFields.StartPoint | CompositionSimpleLinearGradientBrushChangedFields.EndPoint);
		writer.Write(startPoint);
		writer.Write(endPoint);
	}
}
