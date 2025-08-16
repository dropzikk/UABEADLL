using System.Runtime.InteropServices;

namespace Avalonia.Win32.Interop.Automation;

[ComVisible(true)]
[Guid("3589c92c-63f3-4367-99bb-ada653b77cf2")]
[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
public interface ITextProvider
{
	ITextRangeProvider DocumentRange { get; }

	SupportedTextSelection SupportedTextSelection { get; }

	ITextRangeProvider[] GetSelection();

	ITextRangeProvider[] GetVisibleRanges();

	ITextRangeProvider RangeFromChild(IRawElementProviderSimple childElement);

	ITextRangeProvider RangeFromPoint(Point screenLocation);
}
