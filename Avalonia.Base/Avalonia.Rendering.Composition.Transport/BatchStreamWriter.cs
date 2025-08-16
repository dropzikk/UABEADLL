using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Avalonia.Rendering.Composition.Transport;

internal class BatchStreamWriter : IDisposable
{
	private readonly BatchStreamData _output;

	private readonly BatchStreamMemoryPool _memoryPool;

	private readonly BatchStreamObjectPool<object?> _objectPool;

	private BatchStreamSegment<object?[]?> _currentObjectSegment;

	private BatchStreamSegment<IntPtr> _currentDataSegment;

	public BatchStreamWriter(BatchStreamData output, BatchStreamMemoryPool memoryPool, BatchStreamObjectPool<object?> objectPool)
	{
		_output = output;
		_memoryPool = memoryPool;
		_objectPool = objectPool;
	}

	private void CommitDataSegment()
	{
		if (_currentDataSegment.Data != IntPtr.Zero)
		{
			_output.Structs.Enqueue(_currentDataSegment);
		}
		_currentDataSegment = default(BatchStreamSegment<IntPtr>);
	}

	private void NextDataSegment()
	{
		CommitDataSegment();
		_currentDataSegment.Data = _memoryPool.Get();
	}

	private void CommitObjectSegment()
	{
		if (_currentObjectSegment.Data != null)
		{
			_output.Objects.Enqueue(_currentObjectSegment);
		}
		_currentObjectSegment = default(BatchStreamSegment<object[]>);
	}

	private void NextObjectSegment()
	{
		CommitObjectSegment();
		_currentObjectSegment.Data = _objectPool.Get();
	}

	public unsafe void Write<T>(T item) where T : unmanaged
	{
		int num = Unsafe.SizeOf<T>();
		if (_currentDataSegment.Data == IntPtr.Zero || _currentDataSegment.ElementCount + num > _memoryPool.BufferSize)
		{
			NextDataSegment();
		}
		byte* ptr = (byte*)(void*)_currentDataSegment.Data + _currentDataSegment.ElementCount;
		if (RuntimeInformation.ProcessArchitecture == Architecture.Arm)
		{
			UnalignedMemoryHelper.WriteUnaligned(ptr, item);
		}
		else
		{
			Unsafe.WriteUnaligned(ptr, item);
		}
		_currentDataSegment.ElementCount += num;
	}

	public void WriteObject(object? item)
	{
		if (_currentObjectSegment.Data == null || _currentObjectSegment.ElementCount >= _currentObjectSegment.Data.Length)
		{
			NextObjectSegment();
		}
		_currentObjectSegment.Data[_currentObjectSegment.ElementCount] = item;
		_currentObjectSegment.ElementCount++;
	}

	public void Dispose()
	{
		CommitDataSegment();
		CommitObjectSegment();
	}
}
