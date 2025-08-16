using System;
using Avalonia.Metadata;

namespace Avalonia.Controls.Platform;

[Unstable]
public class MountedVolumeInfo : IEquatable<MountedVolumeInfo>
{
	public string? VolumeLabel { get; set; }

	public string? VolumePath { get; set; }

	public ulong VolumeSizeBytes { get; set; }

	public bool Equals(MountedVolumeInfo? other)
	{
		if (VolumeSizeBytes.Equals(other?.VolumeSizeBytes) && object.Equals(VolumePath, other.VolumePath))
		{
			return (VolumeLabel ?? string.Empty).Equals(other.VolumeLabel ?? string.Empty);
		}
		return false;
	}
}
