using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Jpeg.Components.Encoder;

internal class ComponentProcessor : IDisposable
{
	private readonly Size blockAreaSize;

	private readonly Component component;

	private Block8x8F quantTable;

	public Buffer2D<float> ColorBuffer { get; }

	public ComponentProcessor(MemoryAllocator memoryAllocator, Component component, Size postProcessorBufferSize, Block8x8F quantTable)
	{
		this.component = component;
		this.quantTable = quantTable;
		this.component = component;
		blockAreaSize = component.SubSamplingDivisors * 8;
		ColorBuffer = memoryAllocator.Allocate2DOveraligned<float>(postProcessorBufferSize.Width, postProcessorBufferSize.Height, 8, AllocationOptions.Clean);
	}

	public void CopyColorBufferToBlocks(int spectralStep)
	{
		Buffer2D<Block8x8> spectralBlocks = component.SpectralBlocks;
		int width = ColorBuffer.Width;
		int num = spectralStep * component.SamplingFactors.Height;
		Block8x8F block = default(Block8x8F);
		Size subSamplingDivisors = component.SubSamplingDivisors;
		if (subSamplingDivisors.Width != 1 || subSamplingDivisors.Height != 1)
		{
			PackColorBuffer();
		}
		int height = component.SamplingFactors.Height;
		for (int i = 0; i < height; i++)
		{
			int y = i * blockAreaSize.Height;
			Span<float> span = ColorBuffer.DangerousGetRowSpan(y);
			Span<Block8x8> span2 = spectralBlocks.DangerousGetRowSpan(num + i);
			for (int j = 0; j < spectralBlocks.Width; j++)
			{
				int index = j * 8;
				block.ScaledCopyFrom(ref span[index], width);
				block.AddInPlace(-128f);
				FloatingPointDCT.TransformFDCT(ref block);
				Block8x8F.Quantize(ref block, ref span2[j], ref quantTable);
			}
		}
	}

	public Span<float> GetColorBufferRowSpan(int row)
	{
		return ColorBuffer.DangerousGetRowSpan(row);
	}

	public void Dispose()
	{
		ColorBuffer.Dispose();
	}

	private void PackColorBuffer()
	{
		Size subSamplingDivisors = component.SubSamplingDivisors;
		int length = ColorBuffer.Width / subSamplingDivisors.Width;
		float multiplier2 = 1f / (float)(subSamplingDivisors.Width * subSamplingDivisors.Height);
		for (int i = 0; i < ColorBuffer.Height; i += subSamplingDivisors.Height)
		{
			Span<float> target2 = ColorBuffer.DangerousGetRowSpan(i);
			for (int j = 1; j < subSamplingDivisors.Height; j++)
			{
				SumVertical(target2, ColorBuffer.DangerousGetRowSpan(i + j));
			}
			SumHorizontal(target2, subSamplingDivisors.Width);
			MultiplyToAverage(target2, multiplier2);
			target2.Slice(0, length).CopyTo(ColorBuffer.DangerousGetRowSpan(i / subSamplingDivisors.Height));
		}
		static void MultiplyToAverage(Span<float> target, float multiplier)
		{
			if (Avx.IsSupported)
			{
				ref Vector256<float> source9 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(target));
				nuint num15 = target.Vector256Count<float>();
				Vector256<float> right = Vector256.Create(multiplier);
				for (nuint num16 = 0u; num16 < num15; num16++)
				{
					Unsafe.Add(ref source9, num16) = Avx.Multiply(Unsafe.Add(ref source9, num16), right);
				}
			}
			else if (AdvSimd.IsSupported)
			{
				ref Vector128<float> source10 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(target));
				nuint num17 = target.Vector128Count<float>();
				Vector128<float> right2 = Vector128.Create(multiplier);
				for (nuint num18 = 0u; num18 < num17; num18++)
				{
					Unsafe.Add(ref source10, num18) = AdvSimd.Multiply(Unsafe.Add(ref source10, num18), right2);
				}
			}
			else
			{
				ref Vector<float> source11 = ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(target));
				nuint num19 = target.VectorCount<float>();
				Vector<float> vector2 = new Vector<float>(multiplier);
				for (nuint num20 = 0u; num20 < num19; num20++)
				{
					Unsafe.Add(ref source11, num20) *= vector2;
				}
				ref float reference3 = ref MemoryMarshal.GetReference(target);
				for (nuint num21 = num19 * (uint)Vector<float>.Count; num21 < (uint)target.Length; num21++)
				{
					Unsafe.Add(ref reference3, num21) *= multiplier;
				}
			}
		}
		static void SumHorizontal(Span<float> target, int factor)
		{
			Span<float> span = target;
			if (Avx2.IsSupported)
			{
				ref Vector256<float> source8 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(target));
				uint num8 = (uint)factor / 2u;
				int num9 = target.Length % (Vector<float>.Count * factor);
				int num10 = target.Length - num9;
				span = span.Slice(num10);
				target = target.Slice(num10 / factor);
				nuint num11 = Numerics.Vector256Count<float>(num10);
				for (uint num12 = 0u; num12 < num8; num12++)
				{
					num11 /= 2;
					for (nuint num13 = 0u; num13 < num11; num13++)
					{
						nuint num14 = num13 * 2;
						nuint elementOffset = num14 + 1;
						Vector256<float> vector = Avx.HorizontalAdd(Unsafe.Add(ref source8, num14), Unsafe.Add(ref source8, elementOffset));
						Unsafe.Add(ref source8, num13) = Avx2.Permute4x64(vector.AsDouble(), 216).AsSingle();
					}
				}
			}
			for (int k = 0; k < span.Length / factor; k++)
			{
				target[k] = span[k * factor];
				for (int l = 1; l < factor; l++)
				{
					target[k] += span[k * factor + l];
				}
			}
		}
		static void SumVertical(Span<float> target, Span<float> source)
		{
			if (Avx.IsSupported)
			{
				ref Vector256<float> source2 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(target));
				ref Vector256<float> source3 = ref Unsafe.As<float, Vector256<float>>(ref MemoryMarshal.GetReference(source));
				nuint num = source.Vector256Count<float>();
				for (nuint num2 = 0u; num2 < num; num2++)
				{
					Unsafe.Add(ref source2, num2) = Avx.Add(Unsafe.Add(ref source2, num2), Unsafe.Add(ref source3, num2));
				}
			}
			else if (AdvSimd.IsSupported)
			{
				ref Vector128<float> source4 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(target));
				ref Vector128<float> source5 = ref Unsafe.As<float, Vector128<float>>(ref MemoryMarshal.GetReference(source));
				nuint num3 = source.Vector128Count<float>();
				for (nuint num4 = 0u; num4 < num3; num4++)
				{
					Unsafe.Add(ref source4, num4) = AdvSimd.Add(Unsafe.Add(ref source4, num4), Unsafe.Add(ref source5, num4));
				}
			}
			else
			{
				ref Vector<float> source6 = ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(target));
				ref Vector<float> source7 = ref Unsafe.As<float, Vector<float>>(ref MemoryMarshal.GetReference(source));
				nuint num5 = source.VectorCount<float>();
				for (nuint num6 = 0u; num6 < num5; num6++)
				{
					Unsafe.Add(ref source6, num6) += Unsafe.Add(ref source7, num6);
				}
				ref float reference = ref MemoryMarshal.GetReference(target);
				ref float reference2 = ref MemoryMarshal.GetReference(source);
				for (nuint num7 = num5 * (uint)Vector<float>.Count; num7 < (uint)source.Length; num7++)
				{
					Unsafe.Add(ref reference, num7) += Unsafe.Add(ref reference2, num7);
				}
			}
		}
	}
}
