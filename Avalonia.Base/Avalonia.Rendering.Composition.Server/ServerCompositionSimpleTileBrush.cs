using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleTileBrush : ServerCompositionSimpleBrush
{
	private AlignmentX _alignmentX;

	internal static CompositionProperty s_IdOfAlignmentXProperty = CompositionProperty.Register();

	private AlignmentY _alignmentY;

	internal static CompositionProperty s_IdOfAlignmentYProperty = CompositionProperty.Register();

	private RelativeRect _destinationRect;

	internal static CompositionProperty s_IdOfDestinationRectProperty = CompositionProperty.Register();

	private RelativeRect _sourceRect;

	internal static CompositionProperty s_IdOfSourceRectProperty = CompositionProperty.Register();

	private Stretch _stretch;

	internal static CompositionProperty s_IdOfStretchProperty = CompositionProperty.Register();

	private TileMode _tileMode;

	internal static CompositionProperty s_IdOfTileModeProperty = CompositionProperty.Register();

	public AlignmentX AlignmentX
	{
		get
		{
			return GetValue(s_IdOfAlignmentXProperty, ref _alignmentX);
		}
		set
		{
			bool flag = false;
			if (_alignmentX != value)
			{
				flag = true;
			}
			SetValue(s_IdOfAlignmentXProperty, ref _alignmentX, value);
		}
	}

	public AlignmentY AlignmentY
	{
		get
		{
			return GetValue(s_IdOfAlignmentYProperty, ref _alignmentY);
		}
		set
		{
			bool flag = false;
			if (_alignmentY != value)
			{
				flag = true;
			}
			SetValue(s_IdOfAlignmentYProperty, ref _alignmentY, value);
		}
	}

	public RelativeRect DestinationRect
	{
		get
		{
			return GetValue(s_IdOfDestinationRectProperty, ref _destinationRect);
		}
		set
		{
			bool flag = false;
			if (_destinationRect != value)
			{
				flag = true;
			}
			SetValue(s_IdOfDestinationRectProperty, ref _destinationRect, value);
		}
	}

	public RelativeRect SourceRect
	{
		get
		{
			return GetValue(s_IdOfSourceRectProperty, ref _sourceRect);
		}
		set
		{
			bool flag = false;
			if (_sourceRect != value)
			{
				flag = true;
			}
			SetValue(s_IdOfSourceRectProperty, ref _sourceRect, value);
		}
	}

	public Stretch Stretch
	{
		get
		{
			return GetValue(s_IdOfStretchProperty, ref _stretch);
		}
		set
		{
			bool flag = false;
			if (_stretch != value)
			{
				flag = true;
			}
			SetValue(s_IdOfStretchProperty, ref _stretch, value);
		}
	}

	public TileMode TileMode
	{
		get
		{
			return GetValue(s_IdOfTileModeProperty, ref _tileMode);
		}
		set
		{
			bool flag = false;
			if (_tileMode != value)
			{
				flag = true;
			}
			SetValue(s_IdOfTileModeProperty, ref _tileMode, value);
		}
	}

	internal ServerCompositionSimpleTileBrush(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		CompositionSimpleTileBrushChangedFields num = reader.Read<CompositionSimpleTileBrushChangedFields>();
		if ((num & CompositionSimpleTileBrushChangedFields.AlignmentX) == CompositionSimpleTileBrushChangedFields.AlignmentX)
		{
			AlignmentX = reader.Read<AlignmentX>();
		}
		if ((num & CompositionSimpleTileBrushChangedFields.AlignmentY) == CompositionSimpleTileBrushChangedFields.AlignmentY)
		{
			AlignmentY = reader.Read<AlignmentY>();
		}
		if ((num & CompositionSimpleTileBrushChangedFields.DestinationRect) == CompositionSimpleTileBrushChangedFields.DestinationRect)
		{
			DestinationRect = reader.Read<RelativeRect>();
		}
		if ((num & CompositionSimpleTileBrushChangedFields.SourceRect) == CompositionSimpleTileBrushChangedFields.SourceRect)
		{
			SourceRect = reader.Read<RelativeRect>();
		}
		if ((num & CompositionSimpleTileBrushChangedFields.Stretch) == CompositionSimpleTileBrushChangedFields.Stretch)
		{
			Stretch = reader.Read<Stretch>();
		}
		if ((num & CompositionSimpleTileBrushChangedFields.TileMode) == CompositionSimpleTileBrushChangedFields.TileMode)
		{
			TileMode = reader.Read<TileMode>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, AlignmentX alignmentX, AlignmentY alignmentY, RelativeRect destinationRect, RelativeRect sourceRect, Stretch stretch, TileMode tileMode)
	{
		writer.Write(CompositionSimpleTileBrushChangedFields.AlignmentX | CompositionSimpleTileBrushChangedFields.AlignmentY | CompositionSimpleTileBrushChangedFields.DestinationRect | CompositionSimpleTileBrushChangedFields.SourceRect | CompositionSimpleTileBrushChangedFields.Stretch | CompositionSimpleTileBrushChangedFields.TileMode);
		writer.Write(alignmentX);
		writer.Write(alignmentY);
		writer.Write(destinationRect);
		writer.Write(sourceRect);
		writer.Write(stretch);
		writer.Write(tileMode);
	}
}
