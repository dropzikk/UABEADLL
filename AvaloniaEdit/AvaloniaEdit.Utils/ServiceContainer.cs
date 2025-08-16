using System;
using System.Collections.Generic;

namespace AvaloniaEdit.Utils;

internal class ServiceContainer : IServiceContainer, IServiceProvider
{
	private readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

	public ServiceContainer()
	{
		_services.Add(typeof(IServiceProvider), this);
		_services.Add(typeof(IServiceContainer), this);
	}

	public object GetService(Type serviceType)
	{
		_services.TryGetValue(serviceType, out var value);
		return value;
	}

	public void AddService(Type serviceType, object serviceInstance)
	{
		_services[serviceType] = serviceInstance;
	}

	public void RemoveService(Type serviceType)
	{
		_services.Remove(serviceType);
	}
}
