using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.InteropServices;
using Avalonia.Compatibility;
using SkiaSharp;

namespace Avalonia.Skia.Metal;

internal class SkiaMetalApi
{
	internal struct GRMtlTextureInfoNative
	{
		public IntPtr Texture;
	}

	private unsafe delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr> _gr_direct_context_make_metal_with_options;

	private unsafe delegate* unmanaged[Stdcall]<int, int, int, GRMtlTextureInfoNative*, IntPtr> _gr_backendrendertarget_new_metal;

	private readonly ConstructorInfo _contextCtor;

	private readonly MethodInfo _contextOptionsToNative;

	private readonly ConstructorInfo _renderTargetCtor;

	[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(GRContext))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicConstructors, typeof(GRBackendRenderTarget))]
	[DynamicDependency(DynamicallyAccessedMemberTypes.NonPublicMethods, typeof(GRContextOptions))]
	public unsafe SkiaMetalApi()
	{
		GC.KeepAlive(new SKPaint());
		IntPtr handle = NativeLibraryEx.Load("libSkiaSharp", typeof(SKPaint).Assembly);
		if (NativeLibraryEx.TryGetExport(handle, "gr_direct_context_make_metal_with_options", out var address))
		{
			_gr_direct_context_make_metal_with_options = (delegate* unmanaged[Stdcall]<IntPtr, IntPtr, IntPtr, IntPtr>)(void*)address;
			if (NativeLibraryEx.TryGetExport(handle, "gr_backendrendertarget_new_metal", out address))
			{
				_gr_backendrendertarget_new_metal = (delegate* unmanaged[Stdcall]<int, int, int, GRMtlTextureInfoNative*, IntPtr>)(void*)address;
				_contextCtor = typeof(GRContext).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(IntPtr),
					typeof(bool)
				}, null) ?? throw new MissingMemberException("GRContext.ctor(IntPtr,bool)");
				_renderTargetCtor = typeof(GRBackendRenderTarget).GetConstructor(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic, null, new Type[2]
				{
					typeof(IntPtr),
					typeof(bool)
				}, null) ?? throw new MissingMemberException("GRContext.ctor(IntPtr,bool)");
				_contextOptionsToNative = typeof(GRContextOptions).GetMethod("ToNative", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic) ?? throw new MissingMemberException("GRContextOptions.ToNative()");
				return;
			}
			throw new InvalidOperationException("Unable to export gr_backendrendertarget_new_metal. Make sure SkiaSharp is up to date.");
		}
		throw new InvalidOperationException("Unable to export gr_direct_context_make_metal_with_options. Make sure SkiaSharp is up to date.");
	}

	public unsafe GRContext CreateContext(IntPtr device, IntPtr queue, GRContextOptions? options)
	{
		if (options == null)
		{
			options = new GRContextOptions();
		}
		object? structure = _contextOptionsToNative.Invoke(options, null);
		IntPtr intPtr = Marshal.AllocHGlobal(Marshal.SizeOf(structure));
		Marshal.StructureToPtr(structure, intPtr, fDeleteOld: false);
		IntPtr intPtr2 = _gr_direct_context_make_metal_with_options(device, queue, intPtr);
		Marshal.FreeHGlobal(intPtr);
		if (intPtr2 == IntPtr.Zero)
		{
			throw new ArgumentException();
		}
		return (GRContext)_contextCtor.Invoke(new object[2] { intPtr2, true });
	}

	public unsafe GRBackendRenderTarget CreateBackendRenderTarget(int width, int height, int samples, IntPtr texture)
	{
		GRMtlTextureInfoNative gRMtlTextureInfoNative = default(GRMtlTextureInfoNative);
		gRMtlTextureInfoNative.Texture = texture;
		GRMtlTextureInfoNative gRMtlTextureInfoNative2 = gRMtlTextureInfoNative;
		IntPtr intPtr = _gr_backendrendertarget_new_metal(width, height, samples, &gRMtlTextureInfoNative2);
		if (intPtr == IntPtr.Zero)
		{
			throw new InvalidOperationException("Unable to create GRBackendRenderTarget");
		}
		return (GRBackendRenderTarget)_renderTargetCtor.Invoke(new object[2] { intPtr, true });
	}
}
