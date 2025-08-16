using System;
using System.Collections.Generic;
using Avalonia.Logging;
using Avalonia.Platform;
using Avalonia.Rendering.Composition;

namespace Avalonia.OpenGL.Features;

public class ExternalObjectsOpenGlExtensionFeature : IGlContextExternalObjectsFeature
{
	private class ExternalSemaphore : IGlExternalSemaphore, IDisposable
	{
		private readonly IGlContext _context;

		private readonly ExternalObjectsInterface _ext;

		private uint _semaphore;

		public ExternalSemaphore(IGlContext context, ExternalObjectsInterface ext, uint semaphore)
		{
			_context = context;
			_ext = ext;
			_semaphore = semaphore;
		}

		public void Dispose()
		{
			if (!_context.IsLost)
			{
				using (_context.EnsureCurrent())
				{
					_ext.DeleteSemaphoresEXT(1, ref _semaphore);
				}
				_semaphore = 0u;
			}
		}

		public unsafe void WaitSemaphore(IGlExternalImageTexture texture)
		{
			int textureId = ((ExternalImageTexture)texture).TextureId;
			int num = 38290;
			_ext.WaitSemaphoreEXT(_semaphore, 0u, null, 1u, &textureId, &num);
		}

		public unsafe void SignalSemaphore(IGlExternalImageTexture texture)
		{
			int textureId = ((ExternalImageTexture)texture).TextureId;
			int num = 0;
			_ext.SignalSemaphoreEXT(_semaphore, 0u, null, 1u, &textureId, &num);
		}
	}

	private class ExternalImageTexture : IGlExternalImageTexture, IDisposable
	{
		private readonly IGlContext _context;

		private readonly ExternalObjectsInterface _ext;

		private uint _objectId;

		public int TextureId { get; }

		public int InternalFormat => 32856;

		public PlatformGraphicsExternalImageProperties Properties { get; }

		public ExternalImageTexture(IGlContext context, PlatformGraphicsExternalImageProperties properties, ExternalObjectsInterface ext, uint objectId, int textureId)
		{
			Properties = properties;
			TextureId = textureId;
			_context = context;
			_ext = ext;
			_objectId = objectId;
		}

		public void Dispose()
		{
			if (_context.IsLost)
			{
				return;
			}
			using (_context.EnsureCurrent())
			{
				_context.GlInterface.DeleteTexture(TextureId);
				_ext.DeleteMemoryObjectsEXT(1, ref _objectId);
				_objectId = 0u;
			}
		}

		public void AcquireKeyedMutex(uint key)
		{
			throw new NotSupportedException();
		}

		public void ReleaseKeyedMutex(uint key)
		{
			throw new NotSupportedException();
		}
	}

	private readonly IGlContext _context;

	private readonly ExternalObjectsInterface _ext;

	private readonly List<string> _imageTypes = new List<string>();

	private readonly List<string> _semaphoreTypes = new List<string>();

	public IReadOnlyList<string> SupportedImportableExternalImageTypes => _imageTypes;

	public IReadOnlyList<string> SupportedExportableExternalImageTypes { get; } = Array.Empty<string>();

	public IReadOnlyList<string> SupportedImportableExternalSemaphoreTypes => _semaphoreTypes;

	public IReadOnlyList<string> SupportedExportableExternalSemaphoreTypes { get; } = Array.Empty<string>();

	public byte[]? DeviceLuid { get; }

	public byte[]? DeviceUuid { get; }

	public static ExternalObjectsOpenGlExtensionFeature? TryCreate(IGlContext context)
	{
		List<string> extensions = context.GlInterface.GetExtensions();
		if (extensions.Contains("GL_EXT_memory_object") && extensions.Contains("GL_EXT_semaphore"))
		{
			try
			{
				return new ExternalObjectsOpenGlExtensionFeature(context, extensions);
			}
			catch (Exception ex)
			{
				Logger.TryGet(LogEventLevel.Error, "OpenGL")?.Log("ExternalObjectsOpenGlExtensionFeature", "Unable to initialize EXT_external_objects extension: " + ex);
			}
		}
		return null;
	}

	private unsafe ExternalObjectsOpenGlExtensionFeature(IGlContext context, List<string> extensions)
	{
		_context = context;
		_ext = new ExternalObjectsInterface(_context.GlInterface.GetProcAddress);
		if (_ext.IsGetUnsignedBytei_vEXTAvailable)
		{
			_context.GlInterface.GetIntegerv(38294, out var rv);
			if (rv > 0)
			{
				DeviceUuid = new byte[16];
				fixed (byte* deviceUuid = DeviceUuid)
				{
					_ext.GetUnsignedBytei_vEXT(38295, 0u, deviceUuid);
				}
			}
		}
		if (_ext.IsGetUnsignedBytevEXTAvailable && (extensions.Contains("GL_EXT_memory_object_win32") || extensions.Contains("GL_EXT_semaphore_win32")))
		{
			DeviceLuid = new byte[8];
			fixed (byte* deviceLuid = DeviceLuid)
			{
				_ext.GetUnsignedBytevEXT(38297, deviceLuid);
			}
		}
		if (extensions.Contains("GL_EXT_memory_object_fd") && extensions.Contains("GL_EXT_semaphore_fd"))
		{
			_imageTypes.Add("VulkanOpaquePosixFileDescriptor");
			_semaphoreTypes.Add("VulkanOpaquePosixFileDescriptor");
		}
	}

	public IReadOnlyList<PlatformGraphicsExternalImageFormat> GetSupportedFormatsForExternalMemoryType(string type)
	{
		return new PlatformGraphicsExternalImageFormat[1];
	}

	public IGlExportableExternalImageTexture CreateImage(string type, PixelSize size, PlatformGraphicsExternalImageFormat format)
	{
		throw new NotSupportedException();
	}

	public IGlExportableExternalImageTexture CreateSemaphore(string type)
	{
		throw new NotSupportedException();
	}

	public IGlExternalImageTexture ImportImage(IPlatformHandle handle, PlatformGraphicsExternalImageProperties properties)
	{
		string handleDescriptor = handle.HandleDescriptor;
		if (string.IsNullOrEmpty(handleDescriptor))
		{
			throw new ArgumentException("The handle must have a descriptor", "handle");
		}
		if (!_imageTypes.Contains(handleDescriptor))
		{
			throw new ArgumentException(handleDescriptor + " is not supported", "handle");
		}
		if (handleDescriptor == "VulkanOpaquePosixFileDescriptor")
		{
			while (_context.GlInterface.GetError() != 0)
			{
			}
			_ext.CreateMemoryObjectsEXT(1, out var memoryObjects);
			_ext.ImportMemoryFdEXT(memoryObjects, properties.MemorySize, 38278, handle.Handle.ToInt32());
			int error = _context.GlInterface.GetError();
			if (error != 0)
			{
				throw OpenGlException.GetFormattedException("glImportMemoryFdEXT", error);
			}
			_context.GlInterface.GetIntegerv(32873, out var rv);
			int num = _context.GlInterface.GenTexture();
			_context.GlInterface.BindTexture(3553, num);
			_ext.TexStorageMem2DEXT(3553, 1, 32856, properties.Width, properties.Height, memoryObjects, properties.MemoryOffset);
			error = _context.GlInterface.GetError();
			_context.GlInterface.BindTexture(3553, rv);
			if (error != 0)
			{
				throw OpenGlException.GetFormattedException("glTexStorageMem2DEXT", error);
			}
			return new ExternalImageTexture(_context, properties, _ext, memoryObjects, num);
		}
		throw new ArgumentException(handleDescriptor + " is not supported", "handle");
	}

	public IGlExternalSemaphore ImportSemaphore(IPlatformHandle handle)
	{
		string handleDescriptor = handle.HandleDescriptor;
		if (string.IsNullOrEmpty(handleDescriptor))
		{
			throw new ArgumentException("The handle must have a descriptor", "handle");
		}
		if (!_semaphoreTypes.Contains(handleDescriptor))
		{
			throw new ArgumentException(handleDescriptor + " is not supported");
		}
		if (handleDescriptor == "VulkanOpaquePosixFileDescriptor")
		{
			_ext.GenSemaphoresEXT(1, out var semaphores);
			_ext.ImportSemaphoreFdEXT(semaphores, 38278, handle.Handle.ToInt32());
			return new ExternalSemaphore(_context, _ext, semaphores);
		}
		throw new ArgumentException(handleDescriptor + " is not supported", "handle");
	}

	public CompositionGpuImportedImageSynchronizationCapabilities GetSynchronizationCapabilities(string imageHandleType)
	{
		if (imageHandleType == "VulkanOpaquePosixFileDescriptor")
		{
			return CompositionGpuImportedImageSynchronizationCapabilities.Semaphores;
		}
		return (CompositionGpuImportedImageSynchronizationCapabilities)0;
	}
}
