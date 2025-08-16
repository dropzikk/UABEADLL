namespace AssetsTools.NET;

public class AssetTypeReference
{
	public static readonly AssetTypeReference TERMINUS = new AssetTypeReference("Terminus", "UnityEngine.DMAT", "FAKE_ASM");

	public string ClassName { get; set; }

	public string Namespace { get; set; }

	public string AsmName { get; set; }

	public AssetTypeReference()
	{
	}

	public AssetTypeReference(string className, string nameSpace, string asmName)
	{
		ClassName = className;
		Namespace = nameSpace;
		AsmName = asmName;
	}

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetTypeReference" /> with the provided reader, used in
	/// reading <see cref="T:AssetsTools.NET.AssetsFileMetadata" />.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	public void ReadMetadata(AssetsFileReader reader)
	{
		ClassName = reader.ReadNullTerminated();
		Namespace = reader.ReadNullTerminated();
		AsmName = reader.ReadNullTerminated();
	}

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.AssetTypeReference" /> with the provided reader, used in
	/// reading <see cref="T:AssetsTools.NET.AssetTypeReferencedObject" />.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	public void ReadAsset(AssetsFileReader reader)
	{
		ClassName = reader.ReadCountStringInt32();
		reader.Align();
		Namespace = reader.ReadCountStringInt32();
		reader.Align();
		AsmName = reader.ReadCountStringInt32();
		reader.Align();
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetTypeReference" /> with the provided writer, used in
	/// writing <see cref="T:AssetsTools.NET.AssetsFileMetadata" />.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	public void WriteMetadata(AssetsFileWriter writer)
	{
		writer.WriteNullTerminated(ClassName);
		writer.WriteNullTerminated(Namespace);
		writer.WriteNullTerminated(AsmName);
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.AssetTypeReference" /> with the provided writer, used in
	/// writing <see cref="T:AssetsTools.NET.AssetTypeReferencedObject" />.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	public void WriteAsset(AssetsFileWriter writer)
	{
		writer.WriteCountStringInt32(ClassName);
		writer.Align();
		writer.WriteCountStringInt32(Namespace);
		writer.Align();
		writer.WriteCountStringInt32(AsmName);
		writer.Align();
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AssetTypeReference assetTypeReference))
		{
			return false;
		}
		return ClassName == assetTypeReference.ClassName && Namespace == assetTypeReference.Namespace && AsmName == assetTypeReference.AsmName;
	}

	public override int GetHashCode()
	{
		int num = 17;
		num = num * 23 + ClassName.GetHashCode();
		num = num * 23 + Namespace.GetHashCode();
		return num * 23 + AsmName.GetHashCode();
	}
}
