using System;
using System.Numerics;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

public sealed class IccProfileHeader
{
	public uint Size { get; set; }

	public string CmmType { get; set; }

	public IccVersion Version { get; set; }

	public IccProfileClass Class { get; set; }

	public IccColorSpaceType DataColorSpace { get; set; }

	public IccColorSpaceType ProfileConnectionSpace { get; set; }

	public DateTime CreationDate { get; set; }

	public string FileSignature { get; set; }

	public IccPrimaryPlatformType PrimaryPlatformSignature { get; set; }

	public IccProfileFlag Flags { get; set; }

	public uint DeviceManufacturer { get; set; }

	public uint DeviceModel { get; set; }

	public IccDeviceAttribute DeviceAttributes { get; set; }

	public IccRenderingIntent RenderingIntent { get; set; }

	public Vector3 PcsIlluminant { get; set; }

	public string CreatorSignature { get; set; }

	public IccProfileId Id { get; set; }
}
