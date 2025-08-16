namespace SixLabors.ImageSharp.Memory;

public struct MemoryAllocatorOptions
{
	private int? maximumPoolSizeMegabytes;

	private int? allocationLimitMegabytes;

	public int? MaximumPoolSizeMegabytes
	{
		get
		{
			return maximumPoolSizeMegabytes;
		}
		set
		{
			if (value.HasValue)
			{
				Guard.MustBeGreaterThanOrEqualTo(value.Value, 0, "MaximumPoolSizeMegabytes");
			}
			maximumPoolSizeMegabytes = value;
		}
	}

	public int? AllocationLimitMegabytes
	{
		get
		{
			return allocationLimitMegabytes;
		}
		set
		{
			if (value.HasValue)
			{
				Guard.MustBeGreaterThan(value.Value, 0, "AllocationLimitMegabytes");
			}
			allocationLimitMegabytes = value;
		}
	}
}
