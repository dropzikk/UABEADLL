namespace Avalonia.X11;

internal struct XIKeyClassInfo
{
	public int Type;

	public int Sourceid;

	public int NumKeycodes;

	public unsafe int* Keycodes;
}
