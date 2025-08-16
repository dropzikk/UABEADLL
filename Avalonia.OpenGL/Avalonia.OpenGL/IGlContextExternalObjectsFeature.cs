using System.Collections.Generic;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL;

public interface IGlContextExternalObjectsFeature
{
	IReadOnlyList<string> SupportedImportableExternalImageTypes { get; }

	IReadOnlyList<string> SupportedExportableExternalImageTypes { get; }

	IReadOnlyList<string> SupportedImportableExternalSemaphoreTypes { get; }

	IReadOnlyList<string> SupportedExportableExternalSemaphoreTypes { get; }

	byte[]? DeviceLuid { get; }

	byte[]? DeviceUuid { get; }

	IReadOnlyList<PlatformGraphicsExternalImageFormat> GetSupportedFormatsForExternalMemoryType(string type);

	IGlExportableExternalImageTexture CreateImage(string type, PixelSize size, PlatformGraphicsExternalImageFormat format);

	IGlExportableExternalImageTexture CreateSemaphore(string type);

	IGlExternalImageTexture ImportImage(IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties);

	IGlExternalSemaphore ImportSemaphore(IPlatformHandle handle);

	CompositionGpuImportedImageSynchronizationCapabilities GetSynchronizationCapabilities(string imageHandleType);
}
