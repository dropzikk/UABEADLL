using System;

namespace Avalonia.Metadata;

[AttributeUsage(AttributeTargets.Interface)]
public sealed class NotClientImplementableAttribute : Attribute
{
}
