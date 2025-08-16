using System;
using System.Collections.Generic;
using AssetsTools.NET.Extra;

namespace AssetsTools.NET;

public class ClassPackageTypeTree
{
	public DateTime CreationTime { get; set; }

	public List<UnityVersion> Versions { get; set; }

	public List<ClassPackageClassInfo> ClassInformation { get; set; }

	public ClassPackageCommonString CommonString { get; set; }

	public List<ClassPackageTypeNode> Nodes { get; set; }

	public ClassDatabaseStringTable StringTable { get; set; }

	/// <summary>
	/// Read the <see cref="T:AssetsTools.NET.ClassPackageTypeTree" /> with the provided reader.
	/// </summary>
	/// <param name="reader">The reader to use.</param>
	public void Read(AssetsFileReader reader)
	{
		CreationTime = DateTime.FromBinary(reader.ReadInt64());
		int num = reader.ReadInt32();
		Versions = new List<UnityVersion>(num);
		for (int i = 0; i < num; i++)
		{
			Versions.Add(UnityVersion.FromUInt64(reader.ReadUInt64()));
		}
		int num2 = reader.ReadInt32();
		ClassInformation = new List<ClassPackageClassInfo>();
		for (int j = 0; j < num2; j++)
		{
			ClassPackageClassInfo classPackageClassInfo = new ClassPackageClassInfo();
			classPackageClassInfo.Read(reader);
			ClassInformation.Add(classPackageClassInfo);
		}
		CommonString = new ClassPackageCommonString();
		CommonString.Read(reader);
		int num3 = reader.ReadInt32();
		Nodes = new List<ClassPackageTypeNode>(num3);
		for (int k = 0; k < num3; k++)
		{
			ClassPackageTypeNode classPackageTypeNode = new ClassPackageTypeNode();
			classPackageTypeNode.Read(reader);
			Nodes.Add(classPackageTypeNode);
		}
		StringTable = new ClassDatabaseStringTable();
		StringTable.Read(reader);
	}

	/// <summary>
	/// Write the <see cref="T:AssetsTools.NET.ClassPackageTypeTree" /> with the provided writer.
	/// </summary>
	/// <param name="writer">The writer to use.</param>
	public void Write(AssetsFileWriter writer)
	{
		writer.Write(CreationTime.ToBinary());
		writer.Write(Versions.Count);
		for (int i = 0; i < Versions.Count; i++)
		{
			writer.Write(Versions[i].ToUInt64());
		}
		writer.Write(ClassInformation.Count);
		for (int j = 0; j < ClassInformation.Count; j++)
		{
			ClassInformation[j].Write(writer);
		}
		CommonString.Write(writer);
		writer.Write(Nodes.Count);
		for (int k = 0; k < Nodes.Count; k++)
		{
			Nodes[k].Write(writer);
		}
		StringTable.Write(writer);
	}
}
