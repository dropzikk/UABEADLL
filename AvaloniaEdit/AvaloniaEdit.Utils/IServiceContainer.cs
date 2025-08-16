using System;

namespace AvaloniaEdit.Utils;

public interface IServiceContainer : IServiceProvider
{
	void AddService(Type serviceType, object serviceInstance);

	void RemoveService(Type serviceType);
}
