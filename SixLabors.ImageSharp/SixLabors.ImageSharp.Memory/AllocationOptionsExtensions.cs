namespace SixLabors.ImageSharp.Memory;

internal static class AllocationOptionsExtensions
{
	public static bool Has(this AllocationOptions options, AllocationOptions flag)
	{
		return (options & flag) == flag;
	}
}
