using System.Collections.Generic;
using Avalonia.Metadata;
using Avalonia.Platform;

namespace Avalonia.Rendering.Composition;

[NotClientImplementable]
public interface ICompositionGpuInterop
{
	IReadOnlyList<string> SupportedImageHandleTypes { get; }

	IReadOnlyList<string> SupportedSemaphoreTypes { get; }

	bool IsLost { get; }

	byte[]? DeviceLuid { get; set; }

	byte[]? DeviceUuid { get; set; }

	CompositionGpuImportedImageSynchronizationCapabilities GetSynchronizationCapabilities(string imageHandleType);

	ICompositionImportedGpuImage ImportImage(IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties);

	ICompositionImportedGpuImage ImportImage(ICompositionImportableSharedGpuContextImage image);

	ICompositionImportedGpuSemaphore ImportSemaphore(IPlatformHandle handle);

	ICompositionImportedGpuImage ImportSemaphore(ICompositionImportableSharedGpuContextSemaphore image);
}
