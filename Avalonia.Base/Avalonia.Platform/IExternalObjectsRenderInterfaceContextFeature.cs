using System.Collections.Generic;
using Avalonia.Metadata;
using Avalonia.Rendering.Composition;

namespace Avalonia.Platform;

[Unstable]
public interface IExternalObjectsRenderInterfaceContextFeature
{
	IReadOnlyList<string> SupportedImageHandleTypes { get; }

	IReadOnlyList<string> SupportedSemaphoreTypes { get; }

	byte[]? DeviceUuid { get; }

	byte[]? DeviceLuid { get; }

	IPlatformRenderInterfaceImportedImage ImportImage(IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties);

	IPlatformRenderInterfaceImportedImage ImportImage(ICompositionImportableSharedGpuContextImage image);

	IPlatformRenderInterfaceImportedSemaphore ImportSemaphore(IPlatformHandle handle);

	CompositionGpuImportedImageSynchronizationCapabilities GetSynchronizationCapabilities(string imageHandleType);
}
