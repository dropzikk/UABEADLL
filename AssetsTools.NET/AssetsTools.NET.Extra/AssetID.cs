using System;

namespace AssetsTools.NET.Extra;

[Obsolete("use AssetPPtr")]
public class AssetID
{
	public string fileName;

	public long pathID;

	public AssetID(string fileName, long pathID)
	{
		this.fileName = fileName;
		this.pathID = pathID;
	}

	public override bool Equals(object obj)
	{
		if (!(obj is AssetID))
		{
			return false;
		}
		AssetID assetID = (AssetID)obj;
		return assetID.fileName == fileName && assetID.pathID == pathID;
	}

	public override int GetHashCode()
	{
		int num = 17;
		num = num * 23 + fileName.GetHashCode();
		return num * 23 + pathID.GetHashCode();
	}
}
