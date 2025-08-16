using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccCrdInfoTagDataEntry : IccTagDataEntry, IEquatable<IccCrdInfoTagDataEntry>
{
	public string PostScriptProductName { get; }

	public string RenderingIntent0Crd { get; }

	public string RenderingIntent1Crd { get; }

	public string RenderingIntent2Crd { get; }

	public string RenderingIntent3Crd { get; }

	public IccCrdInfoTagDataEntry(string postScriptProductName, string renderingIntent0Crd, string renderingIntent1Crd, string renderingIntent2Crd, string renderingIntent3Crd)
		: this(postScriptProductName, renderingIntent0Crd, renderingIntent1Crd, renderingIntent2Crd, renderingIntent3Crd, IccProfileTag.Unknown)
	{
	}

	public IccCrdInfoTagDataEntry(string postScriptProductName, string renderingIntent0Crd, string renderingIntent1Crd, string renderingIntent2Crd, string renderingIntent3Crd, IccProfileTag tagSignature)
		: base(IccTypeSignature.CrdInfo, tagSignature)
	{
		PostScriptProductName = postScriptProductName;
		RenderingIntent0Crd = renderingIntent0Crd;
		RenderingIntent1Crd = renderingIntent1Crd;
		RenderingIntent2Crd = renderingIntent2Crd;
		RenderingIntent3Crd = renderingIntent3Crd;
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccCrdInfoTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccCrdInfoTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other) && string.Equals(PostScriptProductName, other.PostScriptProductName, StringComparison.OrdinalIgnoreCase) && string.Equals(RenderingIntent0Crd, other.RenderingIntent0Crd, StringComparison.OrdinalIgnoreCase) && string.Equals(RenderingIntent1Crd, other.RenderingIntent1Crd, StringComparison.OrdinalIgnoreCase) && string.Equals(RenderingIntent2Crd, other.RenderingIntent2Crd, StringComparison.OrdinalIgnoreCase))
		{
			return string.Equals(RenderingIntent3Crd, other.RenderingIntent3Crd, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccCrdInfoTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, PostScriptProductName, RenderingIntent0Crd, RenderingIntent1Crd, RenderingIntent2Crd, RenderingIntent3Crd);
	}
}
