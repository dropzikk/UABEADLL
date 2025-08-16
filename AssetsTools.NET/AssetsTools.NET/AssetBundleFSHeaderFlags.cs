namespace AssetsTools.NET;

public enum AssetBundleFSHeaderFlags
{
	None = 0,
	LZMACompressed = 1,
	LZ4Compressed = 2,
	LZ4HCCompressed = 3,
	CompressionMask = 63,
	HasDirectoryInfo = 64,
	BlockAndDirAtEnd = 128,
	OldWebPluginCompatibility = 256,
	BlockInfoNeedPaddingAtStart = 512
}
