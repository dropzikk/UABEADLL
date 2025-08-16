namespace Avalonia.X11;

internal struct XIEventMask
{
	public int Deviceid;

	public int MaskLen;

	public unsafe int* Mask;
}
