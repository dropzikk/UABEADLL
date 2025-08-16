using System;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;

namespace SixLabors.ImageSharp.Metadata.Profiles.Exif;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal sealed class ExifTagDescriptionAttribute : Attribute
{
	public ExifTagDescriptionAttribute(object value, string description)
	{
	}

	public static bool TryGetDescription(ExifTag tag, object? value, [NotNullWhen(true)] out string? description)
	{
		ExifTagValue exifTagValue = (ExifTagValue)(ushort)tag;
		FieldInfo field = typeof(ExifTagValue).GetField(exifTagValue.ToString(), BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);
		description = null;
		if ((object)field == null)
		{
			return false;
		}
		foreach (CustomAttributeData customAttribute in field.CustomAttributes)
		{
			if (object.Equals(customAttribute.ConstructorArguments[0].Value, value))
			{
				description = (string)customAttribute.ConstructorArguments[1].Value;
				return description != null;
			}
		}
		return false;
	}
}
