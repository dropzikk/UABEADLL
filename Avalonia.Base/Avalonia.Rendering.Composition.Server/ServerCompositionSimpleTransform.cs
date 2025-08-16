using System;
using Avalonia.Media;
using Avalonia.Rendering.Composition.Transport;

namespace Avalonia.Rendering.Composition.Server;

internal class ServerCompositionSimpleTransform : SimpleServerRenderResource, ITransform
{
	private Matrix _value;

	internal static CompositionProperty s_IdOfValueProperty = CompositionProperty.Register();

	public Matrix Value
	{
		get
		{
			return GetValue(s_IdOfValueProperty, ref _value);
		}
		set
		{
			bool flag = false;
			if (_value != value)
			{
				flag = true;
			}
			SetValue(s_IdOfValueProperty, ref _value, value);
		}
	}

	internal ServerCompositionSimpleTransform(ServerCompositor compositor)
		: base(compositor)
	{
	}

	protected override void DeserializeChangesCore(BatchStreamReader reader, TimeSpan committedAt)
	{
		base.DeserializeChangesCore(reader, committedAt);
		if ((reader.Read<CompositionSimpleTransformChangedFields>() & CompositionSimpleTransformChangedFields.Value) == CompositionSimpleTransformChangedFields.Value)
		{
			Value = reader.Read<Matrix>();
		}
	}

	internal static void SerializeAllChanges(BatchStreamWriter writer, Matrix value)
	{
		writer.Write(CompositionSimpleTransformChangedFields.Value);
		writer.Write(value);
	}
}
