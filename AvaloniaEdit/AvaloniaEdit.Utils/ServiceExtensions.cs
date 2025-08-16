using System;

namespace AvaloniaEdit.Utils;

public static class ServiceExtensions
{
	public static T GetService<T>(this IServiceProvider provider) where T : class
	{
		return provider.GetService(typeof(T)) as T;
	}

	public static void AddService<T>(this IServiceContainer container, T serviceInstance)
	{
		if (container == null)
		{
			throw new ArgumentNullException("container");
		}
		container.AddService(typeof(T), serviceInstance);
	}

	public static void RemoveService<T>(this IServiceContainer container)
	{
		if (container == null)
		{
			throw new ArgumentNullException("container");
		}
		container.RemoveService(typeof(T));
	}
}
