using System;

namespace SixLabors.ImageSharp.Metadata.Profiles.Icc;

internal sealed class IccSignatureTagDataEntry : IccTagDataEntry, IEquatable<IccSignatureTagDataEntry>
{
	public string SignatureData { get; }

	public IccSignatureTagDataEntry(string signatureData)
		: this(signatureData, IccProfileTag.Unknown)
	{
	}

	public IccSignatureTagDataEntry(string signatureData, IccProfileTag tagSignature)
		: base(IccTypeSignature.Signature, tagSignature)
	{
		SignatureData = signatureData ?? throw new ArgumentNullException("signatureData");
	}

	public override bool Equals(IccTagDataEntry? other)
	{
		if (other is IccSignatureTagDataEntry other2)
		{
			return Equals(other2);
		}
		return false;
	}

	public bool Equals(IccSignatureTagDataEntry? other)
	{
		if (other == null)
		{
			return false;
		}
		if (this == other)
		{
			return true;
		}
		if (base.Equals(other))
		{
			return string.Equals(SignatureData, other.SignatureData, StringComparison.OrdinalIgnoreCase);
		}
		return false;
	}

	public override bool Equals(object? obj)
	{
		if (obj is IccSignatureTagDataEntry other)
		{
			return Equals(other);
		}
		return false;
	}

	public override int GetHashCode()
	{
		return HashCode.Combine(base.Signature, SignatureData);
	}
}
