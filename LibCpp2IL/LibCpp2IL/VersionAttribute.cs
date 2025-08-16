using System;

namespace LibCpp2IL;

[AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
internal class VersionAttribute : Attribute
{
	public float Min { get; set; }

	public float Max { get; set; } = 99f;
}
