namespace SevenZip;

/// <summary>
/// Provides the fields that represent properties idenitifiers for compressing.
/// </summary>
public enum CoderPropID
{
	/// <summary>
	/// Specifies default property.
	/// </summary>
	DefaultProp,
	/// <summary>
	/// Specifies size of dictionary.
	/// </summary>
	DictionarySize,
	/// <summary>
	/// Specifies size of memory for PPM*.
	/// </summary>
	UsedMemorySize,
	/// <summary>
	/// Specifies order for PPM methods.
	/// </summary>
	Order,
	/// <summary>
	/// Specifies Block Size.
	/// </summary>
	BlockSize,
	PosStateBits,
	LitContextBits,
	LitPosBits,
	/// <summary>
	/// Specifies number of fast bytes for LZ*.
	/// </summary>
	NumFastBytes,
	/// <summary>
	/// Specifies match finder. LZMA: "BT2", "BT4" or "BT4B".
	/// </summary>
	MatchFinder,
	/// <summary>
	/// Specifies the number of match finder cyckes.
	/// </summary>
	MatchFinderCycles,
	/// <summary>
	/// Specifies number of passes.
	/// </summary>
	NumPasses,
	/// <summary>
	/// Specifies number of algorithm.
	/// </summary>
	Algorithm,
	/// <summary>
	/// Specifies the number of threads.
	/// </summary>
	NumThreads,
	/// <summary>
	/// Specifies mode with end marker.
	/// </summary>
	EndMarker
}
