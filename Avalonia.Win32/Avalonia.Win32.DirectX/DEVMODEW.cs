namespace Avalonia.Win32.DirectX;

internal struct DEVMODEW
{
	public unsafe fixed ushort dmDeviceName[32];

	public short dmSpecVersion;

	public short dmDriverVersion;

	public short dmSize;

	public short dmDriverExtra;

	public int dmFields;

	public short dmOrientation;

	public short dmPaperSize;

	public short dmPaperLength;

	public short dmPaperWidth;

	public short dmScale;

	public short dmCopies;

	public short dmDefaultSource;

	public short dmPrintQuality;

	public short dmColor;

	public short dmDuplex;

	public short dmYResolution;

	public short dmTTOption;

	public short dmCollate;

	public unsafe fixed ushort dmFormName[32];

	public short dmUnusedPadding;

	public short dmBitsPerPel;

	public int dmPelsWidth;

	public int dmPelsHeight;

	public int dmDisplayFlags;

	public int dmDisplayFrequency;
}
