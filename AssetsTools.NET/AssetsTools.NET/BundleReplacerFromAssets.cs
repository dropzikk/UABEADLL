using System.Collections.Generic;

namespace AssetsTools.NET;

public class BundleReplacerFromAssets : BundleReplacer
{
	private readonly string oldName;

	private readonly string newName;

	private readonly int bundleListIndex;

	private AssetsFile assetsFile;

	private readonly List<AssetsReplacer> assetReplacers;

	private ClassDatabaseFile typeMeta;

	public BundleReplacerFromAssets(string oldName, string newName, AssetsFile assetsFile, List<AssetsReplacer> assetReplacers, int bundleListIndex = -1, ClassDatabaseFile typeMeta = null)
	{
		this.oldName = oldName;
		this.newName = newName ?? oldName;
		this.assetsFile = assetsFile;
		this.assetReplacers = assetReplacers;
		this.bundleListIndex = bundleListIndex;
		this.typeMeta = typeMeta;
	}

	public override BundleReplacementType GetReplacementType()
	{
		return BundleReplacementType.AddOrModify;
	}

	public override int GetBundleListIndex()
	{
		return bundleListIndex;
	}

	public override string GetOriginalEntryName()
	{
		return oldName;
	}

	public override string GetEntryName()
	{
		return newName;
	}

	public override bool Init(AssetsFileReader entryReader, long entryPos, long entrySize, ClassDatabaseFile typeMeta = null)
	{
		if (assetsFile != null)
		{
			return true;
		}
		this.typeMeta = typeMeta;
		if (entryReader == null)
		{
			return false;
		}
		SegmentStream stream = new SegmentStream(entryReader.BaseStream, entryPos, entrySize);
		AssetsFileReader reader = new AssetsFileReader(stream);
		assetsFile = new AssetsFile();
		assetsFile.Read(reader);
		return true;
	}

	public override void Uninit()
	{
		assetsFile.Close();
	}

	public override long Write(AssetsFileWriter writer)
	{
		SegmentStream stream = new SegmentStream(writer.BaseStream, writer.Position);
		AssetsFileWriter writer2 = new AssetsFileWriter(stream);
		assetsFile.Write(writer2, -1L, assetReplacers, typeMeta);
		writer.Position = writer.BaseStream.Length;
		return writer.Position;
	}

	public override long WriteReplacer(AssetsFileWriter writer)
	{
		writer.Write((short)4);
		writer.Write((byte)1);
		if (oldName != null)
		{
			writer.Write((byte)1);
			writer.WriteCountStringInt16(oldName);
		}
		else
		{
			writer.Write((byte)0);
		}
		if (newName != null)
		{
			writer.Write((byte)1);
			writer.WriteCountStringInt16(newName);
		}
		else
		{
			writer.Write((byte)0);
		}
		writer.Write((byte)1);
		writer.Write((long)assetReplacers.Count);
		foreach (AssetsReplacer assetReplacer in assetReplacers)
		{
			assetReplacer.WriteReplacer(writer);
		}
		return writer.Position;
	}

	public override bool HasSerializedData()
	{
		return true;
	}
}
