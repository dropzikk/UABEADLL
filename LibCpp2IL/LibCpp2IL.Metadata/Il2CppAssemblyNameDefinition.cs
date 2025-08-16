using System;
using System.Linq;

namespace LibCpp2IL.Metadata;

public class Il2CppAssemblyNameDefinition
{
	public int nameIndex;

	public int cultureIndex;

	[Version(Max = 24.1f)]
	[Version(Min = 24.2f, Max = 24.3f)]
	public int hashValueIndex;

	public int publicKeyIndex;

	public uint hash_alg;

	public int hash_len;

	public uint flags;

	public int major;

	public int minor;

	public int build;

	public int revision;

	public ulong publicKeyToken;

	public string Name => LibCpp2IlMain.TheMetadata.GetStringFromIndex(nameIndex);

	public string Culture => LibCpp2IlMain.TheMetadata.GetStringFromIndex(cultureIndex);

	public string PublicKey => LibCpp2IlMain.TheMetadata.GetStringFromIndex(publicKeyIndex);

	public string HashValue
	{
		get
		{
			if (!(LibCpp2IlMain.MetadataVersion > 24.3f))
			{
				return LibCpp2IlMain.TheMetadata.GetStringFromIndex(hashValueIndex);
			}
			return "NULL";
		}
	}

	public override string ToString()
	{
		string text = string.Join("-", from b in BitConverter.GetBytes(publicKeyToken)
			select b.ToString("X2"));
		return $"{Name}, Version={major}.{minor}.{build}.{revision}, PublicKeyToken={text}";
	}
}
