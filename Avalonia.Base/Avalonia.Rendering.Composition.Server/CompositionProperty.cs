using System.Threading;

namespace Avalonia.Rendering.Composition.Server;

internal class CompositionProperty
{
	private static volatile int s_NextId = 1;

	public int Id { get; private set; }

	public static CompositionProperty Register()
	{
		return new CompositionProperty
		{
			Id = Interlocked.Increment(ref s_NextId)
		};
	}
}
