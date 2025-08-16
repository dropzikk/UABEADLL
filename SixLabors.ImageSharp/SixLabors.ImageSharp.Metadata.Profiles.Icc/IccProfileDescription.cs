using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal readonly struct IccProfileDescription : IEquatable<IccProfileDescription>
{
	public uint DeviceManufacturer { get; }

	public uint DeviceModel { get; }

	public IccDeviceAttribute DeviceAttributes { get; }

	public IccProfileTag TechnologyInformation { get; }

	public IccLocalizedString[] DeviceManufacturerInfo { get; }

	public IccLocalizedString[] DeviceModelInfo { get; }

	public IccProfileDescription(uint deviceManufacturer, uint deviceModel, IccDeviceAttribute deviceAttributes, IccProfileTag technologyInformation, IccLocalizedString[] deviceManufacturerInfo, IccLocalizedString[] deviceModelInfo)
	{
		DeviceManufacturer = deviceManufacturer;
		DeviceModel = deviceModel;
		DeviceAttributes = deviceAttributes;
		TechnologyInformation = technologyInformation;
		DeviceManufacturerInfo = deviceManufacturerInfo ?? throw new ArgumentNullException("deviceManufacturerInfo");
		DeviceModelInfo = deviceModelInfo ?? throw new ArgumentNullException("deviceModelInfo");
	}

	public bool Equals(IccProfileDescription other)
	{
		if (DeviceManufacturer == other.DeviceManufacturer && DeviceModel == other.DeviceModel && DeviceAttributes == other.DeviceAttributes && TechnologyInformation == other.TechnologyInformation && DeviceManufacturerInfo.AsSpan().SequenceEqual(other.DeviceManufacturerInfo))
		{
			return DeviceModelInfo.AsSpan().SequenceEqual(other.DeviceModelInfo);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccProfileDescription other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(DeviceManufacturer, DeviceModel, DeviceAttributes, TechnologyInformation, DeviceManufacturerInfo, DeviceModelInfo);
	}
}
