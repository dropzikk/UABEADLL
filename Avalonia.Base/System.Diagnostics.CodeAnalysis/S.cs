namespace System.Diagnostics.CodeAnalysis;

internal struct S
{
	private int field;

	[UnscopedRef]
	private ref int Prop1 => ref field;
}
