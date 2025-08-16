using System;
using System.Linq;
using LibCpp2IL.BinaryStructures;

namespace LibCpp2IL.Metadata;

public class Il2CppGenericParameter
{
	public int ownerIndex;

	public int nameIndex;

	public short constraintsStart;

	public short constraintsCount;

	public ushort num;

	public ushort flags;

	public string? Name => LibCpp2IlMain.TheMetadata?.GetStringFromIndex(nameIndex);

	public Il2CppType[]? ConstraintTypes
	{
		get
		{
			if (constraintsCount != 0)
			{
				return LibCpp2IlMain.TheMetadata?.constraintIndices.Skip(constraintsStart).Take(constraintsCount).Select(LibCpp2IlMain.Binary.GetType)
					.ToArray();
			}
			return Array.Empty<Il2CppType>();
		}
	}

	public int Index { get; internal set; }
}
