namespace AssetsTools.NET;

public abstract class BundleReplacer
{
	public abstract BundleReplacementType GetReplacementType();

	public abstract int GetBundleListIndex();

	public abstract string GetOriginalEntryName();

	public abstract string GetEntryName();

	public abstract bool Init(AssetsFileReader entryReader, long entryPos, long entrySize, ClassDatabaseFile typeMeta = null);

	public abstract void Uninit();

	public abstract long Write(AssetsFileWriter writer);

	public abstract long WriteReplacer(AssetsFileWriter writer);

	public abstract bool HasSerializedData();

	public static BundleReplacer ReadBundleReplacer(AssetsFileReader reader)
	{
		return null;
	}
}
