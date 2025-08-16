namespace Avalonia.Win32.DirectX;

internal struct DXGI_FRAME_STATISTICS
{
	public uint PresentCount;

	public uint PresentRefreshCount;

	public uint SyncRefreshCount;

	public ulong SyncQPCTime;

	public ulong SyncGPUTime;
}
