using System.Threading.Tasks;

namespace SixLabors.ImageSharp;

internal static class ConfigurationExtensions
{
	public static ParallelOptions GetParallelOptions(this Configuration configuration)
	{
		return new ParallelOptions
		{
			MaxDegreeOfParallelism = configuration.MaxDegreeOfParallelism
		};
	}
}
