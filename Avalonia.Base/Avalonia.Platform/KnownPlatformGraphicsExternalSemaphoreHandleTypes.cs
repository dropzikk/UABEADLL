namespace Avalonia.Platform;

public static class KnownPlatformGraphicsExternalSemaphoreHandleTypes
{
	public const string VulkanOpaquePosixFileDescriptor = "VulkanOpaquePosixFileDescriptor";

	public const string VulkanOpaqueNtHandle = "VulkanOpaqueNtHandle";

	public const string VulkanOpaqueKmtHandle = "VulkanOpaqueKmtHandle";

	public const string Direct3D12FenceNtHandle = "Direct3D12FenceNtHandle";
}
