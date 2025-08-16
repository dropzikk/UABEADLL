using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Property, Inherited = true)]
public sealed class AmbientAttribute : Attribute
{
}
