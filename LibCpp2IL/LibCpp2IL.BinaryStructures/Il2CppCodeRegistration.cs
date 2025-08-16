namespace LibCpp2IL.BinaryStructures;

public class Il2CppCodeRegistration
{
	[Version(Max = 24.15f)]
	public ulong methodPointersCount;

	[Version(Max = 24.15f)]
	public ulong methodPointers;

	public ulong reversePInvokeWrapperCount;

	public ulong reversePInvokeWrappers;

	public ulong genericMethodPointersCount;

	public ulong genericMethodPointers;

	[Version(Min = 27.1f)]
	[Version(Min = 24.5f, Max = 24.5f)]
	public ulong genericAdjustorThunks;

	public ulong invokerPointersCount;

	public ulong invokerPointers;

	[Version(Max = 24.5f)]
	public ulong customAttributeCount;

	[Version(Max = 24.5f)]
	public ulong customAttributeGeneratorListAddress;

	public ulong unresolvedVirtualCallCount;

	public ulong unresolvedVirtualCallPointers;

	[Version(Min = 29.1f)]
	public ulong unresolvedInstanceCallPointers;

	[Version(Min = 29.1f)]
	public ulong unresolvedStaticCallPointers;

	public ulong interopDataCount;

	public ulong interopData;

	[Version(Min = 24.3f)]
	public ulong windowsRuntimeFactoryCount;

	[Version(Min = 24.3f)]
	public ulong windowsRuntimeFactoryTable;

	[Version(Min = 24.2f)]
	public ulong codeGenModulesCount;

	[Version(Min = 24.2f)]
	public ulong addrCodeGenModulePtrs;
}
