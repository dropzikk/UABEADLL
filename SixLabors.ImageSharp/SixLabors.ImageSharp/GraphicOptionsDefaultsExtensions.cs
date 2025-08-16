using System;
using SixLabors.ImageSharp.Processing;

namespace SixLabors.ImageSharp;

public static class GraphicOptionsDefaultsExtensions
{
	public static IImageProcessingContext SetGraphicsOptions(this IImageProcessingContext context, Action<GraphicsOptions> optionsBuilder)
	{
		GraphicsOptions graphicsOptions = context.GetGraphicsOptions().DeepClone();
		optionsBuilder(graphicsOptions);
		context.Properties[typeof(GraphicsOptions)] = graphicsOptions;
		return context;
	}

	public static void SetGraphicsOptions(this Configuration configuration, Action<GraphicsOptions> optionsBuilder)
	{
		GraphicsOptions graphicsOptions = configuration.GetGraphicsOptions().DeepClone();
		optionsBuilder(graphicsOptions);
		configuration.Properties[typeof(GraphicsOptions)] = graphicsOptions;
	}

	public static IImageProcessingContext SetGraphicsOptions(this IImageProcessingContext context, GraphicsOptions options)
	{
		context.Properties[typeof(GraphicsOptions)] = options;
		return context;
	}

	public static void SetGraphicsOptions(this Configuration configuration, GraphicsOptions options)
	{
		configuration.Properties[typeof(GraphicsOptions)] = options;
	}

	public static GraphicsOptions GetGraphicsOptions(this IImageProcessingContext context)
	{
		if (context.Properties.TryGetValue(typeof(GraphicsOptions), out object value) && value is GraphicsOptions result)
		{
			return result;
		}
		return context.Configuration.GetGraphicsOptions();
	}

	public static GraphicsOptions GetGraphicsOptions(this Configuration configuration)
	{
		if (configuration.Properties.TryGetValue(typeof(GraphicsOptions), out object value) && value is GraphicsOptions result)
		{
			return result;
		}
		GraphicsOptions graphicsOptions = new GraphicsOptions();
		configuration.Properties[typeof(GraphicsOptions)] = graphicsOptions;
		return graphicsOptions;
	}
}
