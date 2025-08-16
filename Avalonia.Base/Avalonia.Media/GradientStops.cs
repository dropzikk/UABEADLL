using System.Collections.Generic;
using System.Linq;
using Avalonia.Collections;
using Avalonia.Media.Immutable;

namespace Avalonia.Media;

public class GradientStops : AvaloniaList<GradientStop>
{
	public GradientStops()
	{
		base.ResetBehavior = ResetBehavior.Remove;
	}

	public IReadOnlyList<ImmutableGradientStop> ToImmutable()
	{
		return this.Select((GradientStop x) => new ImmutableGradientStop(x.Offset, x.Color)).ToArray();
	}
}
