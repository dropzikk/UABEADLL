using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using MicroCom.Runtime;

namespace Avalonia.Win32.DirectX.Impl;

internal class __MicroComID3D11DeviceProxy : MicroComProxyBase, ID3D11Device, IUnknown, IDisposable
{
	public unsafe D3D_FEATURE_LEVEL FeatureLevel => ((delegate* unmanaged[Stdcall]<void*, D3D_FEATURE_LEVEL>)(*base.PPV)[base.VTableSize + 34])(base.PPV);

	public unsafe ushort CreationFlags => ((delegate* unmanaged[Stdcall]<void*, ushort>)(*base.PPV)[base.VTableSize + 35])(base.PPV);

	public unsafe int DeviceRemovedReason => ((delegate* unmanaged[Stdcall]<void*, int>)(*base.PPV)[base.VTableSize + 36])(base.PPV);

	public unsafe ushort ExceptionMode => ((delegate* unmanaged[Stdcall]<void*, ushort>)(*base.PPV)[base.VTableSize + 39])(base.PPV);

	protected override int VTableSize => base.VTableSize + 40;

	public unsafe IUnknown CreateBuffer(IntPtr pDesc, IntPtr pInitialData)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize])(base.PPV, pDesc, pInitialData, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateBuffer failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateTexture1D(IntPtr pDesc, IntPtr pInitialData)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 1])(base.PPV, pDesc, pInitialData, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateTexture1D failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe ID3D11Texture2D CreateTexture2D(D3D11_TEXTURE2D_DESC* pDesc, IntPtr pInitialData)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 2])(base.PPV, pDesc, pInitialData, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateTexture2D failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<ID3D11Texture2D>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateTexture3D(IntPtr pDesc, IntPtr pInitialData)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 3])(base.PPV, pDesc, pInitialData, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateTexture3D failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateShaderResourceView(IntPtr pResource, IntPtr pDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 4])(base.PPV, pResource, pDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateShaderResourceView failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateUnorderedAccessView(IntPtr pResource, IntPtr pDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 5])(base.PPV, pResource, pDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateUnorderedAccessView failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateRenderTargetView(IntPtr pResource, IntPtr pDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 6])(base.PPV, pResource, pDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateRenderTargetView failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateDepthStencilView(IntPtr pResource, IntPtr pDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 7])(base.PPV, pResource, pDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDepthStencilView failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateInputLayout(IntPtr pInputElementDescs, ushort NumElements, void* pShaderBytecodeWithInputSignature, IntPtr BytecodeLength)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, ushort, void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 8])(base.PPV, pInputElementDescs, NumElements, pShaderBytecodeWithInputSignature, BytecodeLength, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateInputLayout failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateVertexShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 9])(base.PPV, pShaderBytecode, BytecodeLength, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateVertexShader failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateGeometryShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 10])(base.PPV, pShaderBytecode, BytecodeLength, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateGeometryShader failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateGeometryShaderWithStreamOutput(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pSODeclaration, ushort NumEntries, ushort* pBufferStrides, ushort NumStrides, ushort RasterizedStream, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, ushort, void*, ushort, ushort, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 11])(base.PPV, pShaderBytecode, BytecodeLength, pSODeclaration, NumEntries, pBufferStrides, NumStrides, RasterizedStream, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateGeometryShaderWithStreamOutput failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreatePixelShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 12])(base.PPV, pShaderBytecode, BytecodeLength, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreatePixelShader failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateHullShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 13])(base.PPV, pShaderBytecode, BytecodeLength, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateHullShader failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateDomainShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 14])(base.PPV, pShaderBytecode, BytecodeLength, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDomainShader failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateComputeShader(IntPtr pShaderBytecode, IntPtr BytecodeLength, IntPtr pClassLinkage)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 15])(base.PPV, pShaderBytecode, BytecodeLength, pClassLinkage, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateComputeShader failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateClassLinkage()
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, int>)(*base.PPV)[base.VTableSize + 16])(base.PPV, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateClassLinkage failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateBlendState(IntPtr pBlendStateDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 17])(base.PPV, pBlendStateDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateBlendState failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateDepthStencilState(IntPtr pDepthStencilDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 18])(base.PPV, pDepthStencilDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDepthStencilState failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateRasterizerState(IntPtr pRasterizerDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 19])(base.PPV, pRasterizerDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateRasterizerState failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateSamplerState(IntPtr pSamplerDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 20])(base.PPV, pSamplerDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateSamplerState failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateQuery(IntPtr pQueryDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 21])(base.PPV, pQueryDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateQuery failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreatePredicate(IntPtr pPredicateDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 22])(base.PPV, pPredicateDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreatePredicate failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateCounter(IntPtr pCounterDesc)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 23])(base.PPV, pCounterDesc, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateCounter failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown CreateDeferredContext(ushort ContextFlags)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, void*, int>)(*base.PPV)[base.VTableSize + 24])(base.PPV, ContextFlags, &pObject);
		if (num != 0)
		{
			throw new COMException("CreateDeferredContext failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe IUnknown OpenSharedResource(IntPtr hResource, Guid* ReturnedInterface)
	{
		void* pObject = null;
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, void*, void*, int>)(*base.PPV)[base.VTableSize + 25])(base.PPV, hResource, ReturnedInterface, &pObject);
		if (num != 0)
		{
			throw new COMException("OpenSharedResource failed", num);
		}
		return MicroComRuntime.CreateProxyOrNullFor<IUnknown>(pObject, ownsHandle: true);
	}

	public unsafe void CheckFormatSupport(DXGI_FORMAT Format, ushort* pFormatSupport)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, DXGI_FORMAT, void*, int>)(*base.PPV)[base.VTableSize + 26])(base.PPV, Format, pFormatSupport);
		if (num != 0)
		{
			throw new COMException("CheckFormatSupport failed", num);
		}
	}

	public unsafe void CheckMultisampleQualityLevels(DXGI_FORMAT Format, ushort SampleCount, ushort* pNumQualityLevels)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, DXGI_FORMAT, ushort, void*, int>)(*base.PPV)[base.VTableSize + 27])(base.PPV, Format, SampleCount, pNumQualityLevels);
		if (num != 0)
		{
			throw new COMException("CheckMultisampleQualityLevels failed", num);
		}
	}

	public unsafe void CheckCounterInfo(IntPtr pCounterInfo)
	{
		((delegate* unmanaged[Stdcall]<void*, IntPtr, void>)(*base.PPV)[base.VTableSize + 28])(base.PPV, pCounterInfo);
	}

	public unsafe void CheckCounter(IntPtr pDesc, IntPtr pType, IntPtr pActiveCounters, IntPtr szName, ushort* pNameLength, IntPtr szUnits, ushort* pUnitsLength, IntPtr szDescription, ushort* pDescriptionLength)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, IntPtr, IntPtr, IntPtr, IntPtr, void*, IntPtr, void*, IntPtr, void*, int>)(*base.PPV)[base.VTableSize + 29])(base.PPV, pDesc, pType, pActiveCounters, szName, pNameLength, szUnits, pUnitsLength, szDescription, pDescriptionLength);
		if (num != 0)
		{
			throw new COMException("CheckCounter failed", num);
		}
	}

	public unsafe void CheckFeatureSupport(D3D11_FEATURE Feature, void* pFeatureSupportData, ushort FeatureSupportDataSize)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, D3D11_FEATURE, void*, ushort, int>)(*base.PPV)[base.VTableSize + 30])(base.PPV, Feature, pFeatureSupportData, FeatureSupportDataSize);
		if (num != 0)
		{
			throw new COMException("CheckFeatureSupport failed", num);
		}
	}

	public unsafe void GetPrivateData(Guid* guid, ushort* pDataSize, void* pData)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 31])(base.PPV, guid, pDataSize, pData);
		if (num != 0)
		{
			throw new COMException("GetPrivateData failed", num);
		}
	}

	public unsafe void SetPrivateData(Guid* guid, ushort DataSize, IntPtr* pData)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, ushort, void*, int>)(*base.PPV)[base.VTableSize + 32])(base.PPV, guid, DataSize, pData);
		if (num != 0)
		{
			throw new COMException("SetPrivateData failed", num);
		}
	}

	public unsafe void SetPrivateDataInterface(Guid* guid, IUnknown pData)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, void*, void*, int>)(*base.PPV)[base.VTableSize + 33])(base.PPV, guid, MicroComRuntime.GetNativePointer(pData));
		if (num != 0)
		{
			throw new COMException("SetPrivateDataInterface failed", num);
		}
	}

	public unsafe void GetImmediateContext(IntPtr* ppImmediateContext)
	{
		((delegate* unmanaged[Stdcall]<void*, void*, void>)(*base.PPV)[base.VTableSize + 37])(base.PPV, ppImmediateContext);
	}

	public unsafe void SetExceptionMode(ushort RaiseFlags)
	{
		int num = ((delegate* unmanaged[Stdcall]<void*, ushort, int>)(*base.PPV)[base.VTableSize + 38])(base.PPV, RaiseFlags);
		if (num != 0)
		{
			throw new COMException("SetExceptionMode failed", num);
		}
	}

	[ModuleInitializer]
	internal static void __MicroComModuleInit()
	{
		MicroComRuntime.Register(typeof(ID3D11Device), new Guid("db6f6ddb-ac77-4e88-8253-819df9bbf140"), (IntPtr p, bool owns) => new __MicroComID3D11DeviceProxy(p, owns));
	}

	protected __MicroComID3D11DeviceProxy(IntPtr nativePointer, bool ownsHandle)
		: base(nativePointer, ownsHandle)
	{
	}
}
