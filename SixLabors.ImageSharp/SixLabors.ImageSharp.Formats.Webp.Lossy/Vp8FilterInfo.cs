namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8FilterInfo : IDeepCloneable
{
	private byte limit;

	private byte innerLevel;

	private byte highEdgeVarianceThreshold;

	public byte Limit
	{
		get
		{
			return limit;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, (byte)0, (byte)189, "Limit");
			limit = value;
		}
	}

	public byte InnerLevel
	{
		get
		{
			return innerLevel;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, (byte)0, (byte)63, "InnerLevel");
			innerLevel = value;
		}
	}

	public bool UseInnerFiltering { get; set; }

	public byte HighEdgeVarianceThreshold
	{
		get
		{
			return highEdgeVarianceThreshold;
		}
		set
		{
			Guard.MustBeBetweenOrEqualTo(value, (byte)0, (byte)2, "HighEdgeVarianceThreshold");
			highEdgeVarianceThreshold = value;
		}
	}

	public Vp8FilterInfo()
	{
	}

	public Vp8FilterInfo(Vp8FilterInfo other)
	{
		Limit = other.Limit;
		HighEdgeVarianceThreshold = other.HighEdgeVarianceThreshold;
		InnerLevel = other.InnerLevel;
		UseInnerFiltering = other.UseInnerFiltering;
	}

	public IDeepCloneable DeepClone()
	{
		return new Vp8FilterInfo(this);
	}
}
