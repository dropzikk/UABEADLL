using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("f7063da8-8359-439c-9297-bbc5299a7d87")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface IRawElementProviderFragment : IRawElementProviderSimple
{
	Rect BoundingRectangle { get; }

	IRawElementProviderFragmentRoot? FragmentRoot { get; }

	IRawElementProviderFragment? Navigate(NavigateDirection direction);

	int[]? GetRuntimeId();

	IRawElementProviderSimple[]? GetEmbeddedFragmentRoots();

	void SetFocus();
}
