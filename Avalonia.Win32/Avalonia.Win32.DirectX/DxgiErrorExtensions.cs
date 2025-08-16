namespace Avalonia.Win32.DirectX;

internal static class DxgiErrorExtensions
{
	public static bool IsDeviceLostError(this DXGI_ERROR error)
	{
		if (error != DXGI_ERROR.DXGI_ERROR_DEVICE_REMOVED && error != DXGI_ERROR.DXGI_ERROR_DEVICE_HUNG && error != DXGI_ERROR.DXGI_ERROR_DEVICE_RESET)
		{
			return error == DXGI_ERROR.DXGI_ERROR_NOT_CURRENTLY_AVAILABLE;
		}
		return true;
	}
}
