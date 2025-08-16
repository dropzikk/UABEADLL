using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("2acad808-b2d4-452d-a407-91ff1ad167b2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ISelectionItemProvider
{
	bool IsSelected
	{
		[return: MarshalAs(UnmanagedType.Bool)]
		get;
	}

	IRawElementProviderSimple? SelectionContainer { get; }

	void Select();

	void AddToSelection();

	void RemoveFromSelection();
}
