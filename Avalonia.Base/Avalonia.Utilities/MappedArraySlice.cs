using System.Runtime.CompilerServices;

namespace Avalonia.Utilities;

internal readonly struct MappedArraySlice<T> where T : struct
{
	private readonly ArraySlice<T> _data;

	private readonly ArraySlice<int> _map;

	public int Length => _map.Length;

	public ref T this[int index]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get
		{
			return ref _data[_map[index]];
		}
	}

	public MappedArraySlice(in ArraySlice<T> data, in ArraySlice<int> map)
	{
		_data = data;
		_map = map;
	}
}
