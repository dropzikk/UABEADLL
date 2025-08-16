namespace Avalonia.X11;

internal struct XIButtonState
{
	public int MaskLen;

	public unsafe byte* Mask;
}
