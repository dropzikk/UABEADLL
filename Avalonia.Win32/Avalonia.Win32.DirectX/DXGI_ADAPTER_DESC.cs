namespace Avalonia.Win32.DirectX;

internal struct DXGI_ADAPTER_DESC
{
	public unsafe fixed ushort Description[128];

	public uint VendorId;

	public uint DeviceId;

	public uint SubSysId;

	public uint Revision;

	public nuint DedicatedVideoMemory;

	public nuint DedicatedSystemMemory;

	public nuint SharedSystemMemory;

	public ulong AdapterLuid;
}
