namespace LibCpp2IL.BinaryStructures;

public class Il2CppCodeGenModule
{
	public ulong moduleName;

	public long methodPointerCount;

	public ulong methodPointers;

	[Version(Min = 27.1f)]
	[Version(Min = 24.5f, Max = 24.5f)]
	public long adjustorThunkCount;

	[Version(Min = 27.1f)]
	[Version(Min = 24.5f, Max = 24.5f)]
	public ulong adjustorThunks;

	public ulong invokerIndices;

	public ulong reversePInvokeWrapperCount;

	public ulong reversePInvokeWrapperIndices;

	public long rgctxRangesCount;

	public ulong pRgctxRanges;

	public long rgctxsCount;

	public ulong rgctxs;

	public ulong debuggerMetadata;

	[Version(Min = 27f, Max = 27.1f)]
	public ulong customAttributeCacheGenerator;

	[Version(Min = 27f)]
	public ulong moduleInitializer;

	[Version(Min = 27f)]
	public ulong staticConstructorTypeIndices;

	[Version(Min = 27f)]
	public ulong metadataRegistration;

	[Version(Min = 27f)]
	public ulong codeRegistration;

	private string? _cachedName;

	public string Name
	{
		get
		{
			if (_cachedName == null)
			{
				_cachedName = LibCpp2IlMain.Binary.ReadStringToNull(LibCpp2IlMain.Binary.MapVirtualAddressToRaw(moduleName));
			}
			return _cachedName;
		}
	}

	public Il2CppTokenRangePair[] RGCTXRanges => LibCpp2IlMain.Binary.GetRGCTXRangePairsForModule(this);
}
