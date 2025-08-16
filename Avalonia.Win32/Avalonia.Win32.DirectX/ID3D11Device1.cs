using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface ID3D11Device1 : ID3D11Device, IUnknown, IDisposable
{
	unsafe void GetImmediateContext1(void** ppImmediateContext);

	IUnknown CreateDeferredContext1(ushort ContextFlags);

	unsafe IUnknown CreateBlendState1(void* pBlendStateDesc);

	unsafe IUnknown CreateRasterizerState1(void* pRasterizerDesc);

	unsafe IUnknown CreateDeviceContextState(ushort Flags, void* pFeatureLevels, ushort FeatureLevels, ushort SDKVersion, Guid* EmulatedInterface, void* pChosenFeatureLevel);

	unsafe IUnknown OpenSharedResource1(IntPtr hResource, Guid* ReturnedInterface);

	unsafe void OpenSharedResourceByName(ushort* lpName, int dwDesiredAccess, Guid* returnedInterface, void** ppResource);
}
