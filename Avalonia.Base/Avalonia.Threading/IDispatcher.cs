using System;

namespace Avalonia.Threading;

public interface IDispatcher
{
	bool CheckAccess();

	void VerifyAccess();

	void Post(Action action, DispatcherPriority priority = default(DispatcherPriority));
}
