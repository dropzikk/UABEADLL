namespace Avalonia.Styling;

internal interface IStyleInstance
{
	IStyle Source { get; }

	bool HasActivator { get; }

	bool IsActive { get; }
}
