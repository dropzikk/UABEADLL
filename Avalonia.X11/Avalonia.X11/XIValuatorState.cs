namespace Avalonia.X11;

internal struct XIValuatorState
{
	public int MaskLen;

	public unsafe byte* Mask;

	public unsafe double* Values;
}
