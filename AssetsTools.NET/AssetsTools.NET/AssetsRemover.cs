using System.Collections.Generic;

namespace AssetsTools.NET;

public class AssetsRemover : AssetsReplacer
{
	private readonly long pathID;

	public AssetsRemover(long pathID)
	{
		this.pathID = pathID;
	}

	public override AssetsReplacementType GetReplacementType()
	{
		return AssetsReplacementType.Remove;
	}

	public override long GetPathID()
	{
		return pathID;
	}

	public override int GetClassID()
	{
		return 0;
	}

	public override ushort GetMonoScriptID()
	{
		return 0;
	}

	public override void SetMonoScriptID(ushort scriptId)
	{
	}

	public override long GetSize()
	{
		return 0L;
	}

	public override bool GetPropertiesHash(out Hash128 propertiesHash)
	{
		propertiesHash = default(Hash128);
		return false;
	}

	public override bool SetPropertiesHash(Hash128 propertiesHash)
	{
		return false;
	}

	public override bool GetScriptIDHash(out Hash128 scriptIdHash)
	{
		scriptIdHash = default(Hash128);
		return false;
	}

	public override bool SetScriptIDHash(Hash128 scriptIdHash)
	{
		return false;
	}

	public override bool GetTypeInfo(out ClassDatabaseFile file, out ClassDatabaseType type)
	{
		file = null;
		type = null;
		return false;
	}

	public override bool SetTypeInfo(ClassDatabaseFile file, ClassDatabaseType type, bool localCopy)
	{
		return false;
	}

	public override bool GetPreloadDependencies(out List<AssetPPtr> preloadList)
	{
		preloadList = null;
		return false;
	}

	public override bool SetPreloadDependencies(List<AssetPPtr> preloadList)
	{
		return false;
	}

	public override bool AddPreloadDependency(AssetPPtr dependency)
	{
		return false;
	}

	public override long Write(AssetsFileWriter writer)
	{
		return writer.Position;
	}

	public override long WriteReplacer(AssetsFileWriter writer)
	{
		writer.Write((short)0);
		writer.Write((byte)1);
		writer.Write((byte)1);
		writer.Write(0);
		writer.Write(pathID);
		writer.Write(0);
		writer.Write((ushort)0);
		writer.Write(0);
		return writer.Position;
	}

	public override void Dispose()
	{
	}
}
