namespace Avalonia.Input;

internal interface IAccessKeyHandler
{
	IMainMenu? MainMenu { get; set; }

	void SetOwner(IInputRoot owner);

	void Register(char accessKey, IInputElement element);

	void Unregister(IInputElement element);
}
