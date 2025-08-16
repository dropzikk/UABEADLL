using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleSolidColorBrush : ServerCompositionSimpleBrush, ISolidColorBrush, IBrush
{
	private Color _color;

	internal static CompositionProperty s_IdOfColorProperty = CompositionProperty.Register();

	public Color Color
	{
		get
		{
			return GetValue(s_IdOfColorProperty, ref _color);
		}
		set
		{
			bool flag = false;
			if (_color != value)
			{
				flag = true;
			}
			SetValue(s_IdOfColorProperty, ref _color, value);
		}
	}

	internal ServerCompositionSimpleSolidColorBrush(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		if ((reader.Read<CompositionSimpleSolidColorBrushChangedFields>() & CompositionSimpleSolidColorBrushChangedFields.Color) == CompositionSimpleSolidColorBrushChangedFields.Color)
		{
			Color = reader.Read<Color>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, Color color)
	{
		writer.Write(CompositionSimpleSolidColorBrushChangedFields.Color);
		writer.Write(color);
	}
}
