using System;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX;

internal interface ID3D11Device : IUnknown, IDisposable
{
	D3D_FEATURE_LEVEL FeatureLevel { get; }

	ushort CreationFlags { get; }

	int DeviceRemovedReason { get; }

	ushort ExceptionMode { get; }

	IUnknown CreateBuffer(IntPtr pDesc, IntPtr pInitialData);

	IUnknown CreateTexture1D(IntPtr pDesc, IntPtr pInitialData);

	unsafe ID3D11Texture2D CreateTexture2D(D3D11_TEXTURE2D_DESC* pDesc, IntPtr pInitialData);

	IUnknown CreateTexture3D(IntPtr pDesc, IntPtr pInitialData);

	IUnknown CreateShaderResourceView(IntPtr pResource, IntPtr pDesc);

	IUnknown CreateUnorderedAccessView(IntPtr pResource, IntPtr pDesc);

	IUnknown CreateRenderTargetView(IntPtr pResource, IntPtr pDesc);

	IUnknown CreateDepthStencilView(IntPtr pResource, IntPtr pDesc);

	unsafe IUnknown CreateInputLayout(IntPtr pInputElementDescs, ushort NumElements, void* pShaderBytecodeWithInputSignature, IntPtr BytecodeLength);

	IUnknown CreateVertexShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage);

	IUnknown CreateGeometryShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage);

	unsafe IUnknown CreateGeometryShaderWithStreamOutput(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pSODeclaration, ushort NumEntries, ushort* pBufferStrides, ushort NumStrides, ushort RasterizedStream, IntPtr pClassLinkage);

	IUnknown CreatePixelShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage);

	IUnknown CreateHullShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage);

	IUnknown CreateDomainShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage);

	IUnknown CreateComputeShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage);

	IUnknown CreateClassLinkage();

	IUnknown CreateBlendState(IntPtr pBlendStateDesc);

	IUnknown CreateDepthStencilState(IntPtr pDepthStencilDesc);

	IUnknown CreateRasterizerState(IntPtr pRasterizerDesc);

	IUnknown CreateSamplerState(IntPtr pSamplerDesc);

	IUnknown CreateQuery(IntPtr pQueryDesc);

	IUnknown CreatePredicate(IntPtr pPredicateDesc);

	IUnknown CreateCounter(IntPtr pCounterDesc);

	IUnknown CreateDeferredContext(ushort ContextFlags);

	unsafe IUnknown OpenSharedResource(IntPtr hResource, Guid* ReturnedInterface);

	unsafe void CheckFormatSupport(DXGI_FORMAT Format, ushort* pFormatSupport);

	unsafe void CheckMultisampleQualityLevels(DXGI_FORMAT Format, ushort SampleCount, ushort* pNumQualityLevels);

	void CheckCounterInfo(IntPtr pCounterInfo);

	unsafe void CheckCounter(IntPtr pDesc, IntPtr pType, IntPtr pActiveCounters, IntPtr szName, ushort* pNameLength, IntPtr szUnits, ushort* pUnitsLength, IntPtr szDescription, ushort* pDescriptionLength);

	unsafe void CheckFeatureSupport(D3D11_FEATURE Feature, void* pFeatureSupportData, ushort FeatureSupportDataSize);

	unsafe void GetPrivateData(Guid* guid, ushort* pDataSize, void* pData);

	unsafe void SetPrivateData(Guid* guid, ushort DataSize, IntPtr* pData);

	unsafe void SetPrivateDataInterface(Guid* guid, IUnknown pData);

	unsafe void GetImmediateContext(IntPtr* ppImmediateContext);

	void SetExceptionMode(ushort RaiseFlags);
}
