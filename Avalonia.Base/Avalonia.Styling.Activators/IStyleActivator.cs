using System;

namespace Avalonia.Styling.Activators;

internal interface IStyleActivator : IDisposable
{
	bool IsSubscribed { get; }

	bool GetIsActive();

	void Subscribe(IStyleActivatorSink sink);

	void Unsubscribe(IStyleActivatorSink sink);
}
