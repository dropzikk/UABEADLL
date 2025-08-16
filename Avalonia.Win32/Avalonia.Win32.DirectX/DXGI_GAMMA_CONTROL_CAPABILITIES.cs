namespace Avalonia.Win32.DirectX;

internal struct DXGI_GAMMA_CONTROL_CAPABILITIES
{
	public int ScaleAndOffsetSupported;

	public float MaxConvertedValue;

	public float MinConvertedValue;

	public uint NumGammaControlPoints;

	public unsafe fixed float ControlPointPositions[1025];
}
