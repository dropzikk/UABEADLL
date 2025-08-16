using System;
using System.Buffers;
using System.IO;
using System.Runtime.CompilerServices;
using SixLabors.ImageSharp.Common.Helpers;
using SixLabors.ImageSharp.Formats.Webp.BitWriter;
using SixLabors.ImageSharp.Formats.Webp.Chunks;
using SixLabors.ImageSharp.Memory;
using SixLabors.ImageSharp.Metadata;
using SixLabors.ImageSharp.Metadata.Profiles.Exif;
using SixLabors.ImageSharp.Metadata.Profiles.Xmp;
using SixLabors.ImageSharp.PixelFormats;

namespace SixLabors.ImageSharp.Formats.Webp.Lossy;

internal class Vp8Encoder : IDisposable
{
	private readonly MemoryAllocator memoryAllocator;

	private readonly Configuration configuration;

	private readonly uint quality;

	private readonly WebpEncodingMethod method;

	private readonly int entropyPasses;

	private readonly int filterStrength;

	private readonly int spatialNoiseShaping;

	private Vp8BitWriter bitWriter;

	private readonly bool skipMetadata;

	private readonly Vp8RdLevel rdOptLevel;

	private int maxI4HeaderBits;

	private int alpha;

	private int uvAlpha;

	private readonly bool alphaCompression;

	private const int NumMbSegments = 4;

	private const int MaxItersKMeans = 6;

	private const float DqLimit = 0.4f;

	private const ulong Partition0SizeLimit = 1069547520uL;

	private const long HeaderSizeEstimate = 30L;

	private const int QMin = 0;

	private const int QMax = 100;

	private static ReadOnlySpan<byte> AverageBytesPerMb => new byte[8] { 50, 24, 16, 9, 7, 5, 3, 2 };

	public int BaseQuant { get; set; }

	public Vp8EncProba Proba { get; }

	public Vp8EncSegmentHeader SegmentHeader { get; private set; }

	public Vp8SegmentInfo[] SegmentInfos { get; }

	public Vp8MacroBlockInfo[] MbInfo { get; }

	public Vp8FilterHeader FilterHeader { get; }

	public int Alpha { get; set; }

	public int Width { get; }

	public int Height { get; }

	public int PredsWidth { get; }

	public int Mbw { get; }

	public int Mbh { get; }

	public int DqY1Dc { get; private set; }

	public int DqY2Ac { get; private set; }

	public int DqY2Dc { get; private set; }

	public int DqUvAc { get; private set; }

	public int DqUvDc { get; private set; }

	private IMemoryOwner<byte> Y { get; }

	private IMemoryOwner<byte> U { get; }

	private IMemoryOwner<byte> V { get; }

	public byte[] YTop { get; }

	public byte[] UvTop { get; }

	public uint[] Nz { get; }

	public byte[] Preds { get; }

	public sbyte[] TopDerr { get; }

	private int MbHeaderLimit { get; }

	public Vp8Encoder(MemoryAllocator memoryAllocator, Configuration configuration, int width, int height, uint quality, bool skipMetadata, WebpEncodingMethod method, int entropyPasses, int filterStrength, int spatialNoiseShaping, bool alphaCompression)
	{
		this.memoryAllocator = memoryAllocator;
		this.configuration = configuration;
		Width = width;
		Height = height;
		this.quality = Math.Min(quality, 100u);
		this.skipMetadata = skipMetadata;
		this.method = method;
		this.entropyPasses = Numerics.Clamp(entropyPasses, 1, 10);
		this.filterStrength = Numerics.Clamp(filterStrength, 0, 100);
		this.spatialNoiseShaping = Numerics.Clamp(spatialNoiseShaping, 0, 100);
		this.alphaCompression = alphaCompression;
		if (method == WebpEncodingMethod.Level6)
		{
			rdOptLevel = Vp8RdLevel.RdOptTrellisAll;
		}
		else if (method >= WebpEncodingMethod.Level5)
		{
			rdOptLevel = Vp8RdLevel.RdOptTrellis;
		}
		else if (method >= WebpEncodingMethod.Level3)
		{
			rdOptLevel = Vp8RdLevel.RdOptBasic;
		}
		else
		{
			rdOptLevel = Vp8RdLevel.RdOptNone;
		}
		int length = width * height;
		Mbw = width + 15 >> 4;
		Mbh = height + 15 >> 4;
		int length2 = (width + 1 >> 1) * (height + 1 >> 1);
		Y = this.memoryAllocator.Allocate<byte>(length);
		U = this.memoryAllocator.Allocate<byte>(length2);
		V = this.memoryAllocator.Allocate<byte>(length2);
		YTop = new byte[Mbw * 16];
		UvTop = new byte[Mbw * 16 * 2];
		Nz = new uint[Mbw + 1];
		MbHeaderLimit = 1069547520 / (Mbw * Mbh);
		TopDerr = new sbyte[Mbw * 4];
		maxI4HeaderBits = 65536;
		MbInfo = new Vp8MacroBlockInfo[Mbw * Mbh];
		for (int i = 0; i < MbInfo.Length; i++)
		{
			MbInfo[i] = new Vp8MacroBlockInfo();
		}
		SegmentInfos = new Vp8SegmentInfo[4];
		for (int j = 0; j < 4; j++)
		{
			SegmentInfos[j] = new Vp8SegmentInfo();
		}
		FilterHeader = new Vp8FilterHeader();
		int num = (4 * Mbw + 1) * (4 * Mbh + 1) + PredsWidth + 1;
		PredsWidth = 4 * Mbw + 1;
		Proba = new Vp8EncProba();
		Preds = new byte[num + PredsWidth + Mbw];
		Preds.AsSpan().Fill(205);
		Nz.AsSpan().Fill(3452816845u);
		ResetBoundaryPredictions();
	}

	public WebpVp8X EncodeHeader<TPixel>(Image<TPixel> image, Stream stream, bool hasAlpha, bool hasAnimation) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageMetadata metadata = image.Metadata;
		metadata.SyncProfiles();
		ExifProfile exifProfile = (skipMetadata ? null : metadata.ExifProfile);
		XmpProfile xmpProfile = (skipMetadata ? null : metadata.XmpProfile);
		WebpVp8X result = BitWriterBase.WriteTrunksBeforeData(stream, (uint)image.Width, (uint)image.Height, exifProfile, xmpProfile, metadata.IccProfile, hasAlpha, hasAnimation);
		if (hasAnimation)
		{
			WebpMetadata webpMetadata = WebpCommonUtils.GetWebpMetadata(image);
			BitWriterBase.WriteAnimationParameter(stream, webpMetadata.BackgroundColor, webpMetadata.RepeatCount);
		}
		return result;
	}

	public void EncodeFooter<TPixel>(Image<TPixel> image, in WebpVp8X vp8x, bool hasAlpha, Stream stream, long initialPosition) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageMetadata metadata = image.Metadata;
		ExifProfile exifProfile = (skipMetadata ? null : metadata.ExifProfile);
		XmpProfile xmpProfile = (skipMetadata ? null : metadata.XmpProfile);
		bool flag = hasAlpha && vp8x != default(WebpVp8X);
		BitWriterBase.WriteTrunksAfterData(stream, flag ? vp8x.WithAlpha(hasAlpha: true) : vp8x, flag, initialPosition, exifProfile, xmpProfile);
	}

	public bool EncodeAnimation<TPixel>(ImageFrame<TPixel> frame, Stream stream, Rectangle bounds, WebpFrameMetadata frameMetadata) where TPixel : unmanaged, IPixel<TPixel>
	{
		return Encode(stream, frame, bounds, frameMetadata, hasAnimation: true, null);
	}

	public void EncodeStatic<TPixel>(Stream stream, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		ImageFrame<TPixel> rootFrame = image.Frames.RootFrame;
		Encode(stream, rootFrame, image.Bounds, WebpCommonUtils.GetWebpFrameMetadata(rootFrame), hasAnimation: false, image);
	}

	private bool Encode<TPixel>(Stream stream, ImageFrame<TPixel> frame, Rectangle bounds, WebpFrameMetadata frameMetadata, bool hasAnimation, Image<TPixel> image) where TPixel : unmanaged, IPixel<TPixel>
	{
		int width = bounds.Width;
		int height = bounds.Height;
		int num = width * height;
		Span<byte> span = Y.GetSpan();
		Span<byte> span2 = U.GetSpan();
		Span<byte> span3 = V.GetSpan();
		Buffer2DRegion<TPixel> region = frame.PixelBuffer.GetRegion(bounds);
		bool flag = YuvConversion.ConvertRgbToYuv(region, configuration, memoryAllocator, span, span2, span3);
		if (!hasAnimation)
		{
			EncodeHeader(image, stream, flag, hasAnimation: false);
		}
		int num2 = width;
		int uvStride = num2 + 1 >> 1;
		Vp8EncIterator vp8EncIterator = new Vp8EncIterator(this);
		Span<int> span4 = stackalloc int[256];
		alpha = MacroBlockAnalysis(width, height, vp8EncIterator, span, span2, span3, num2, uvStride, span4, out uvAlpha);
		int num3 = Mbw * Mbw;
		alpha /= num3;
		uvAlpha /= num3;
		SegmentHeader = new Vp8EncSegmentHeader(4);
		AssignSegments(span4);
		SetLoopParams(quality);
		int num4 = AverageBytesPerMb[BaseQuant >> 4];
		int expectedSize = Mbw * Mbh * num4;
		bitWriter = new Vp8BitWriter(expectedSize, this);
		StatLoop(width, height, num2, uvStride);
		vp8EncIterator.Init();
		Vp8EncIterator.InitFilter();
		Vp8ModeScore rd = new Vp8ModeScore();
		Vp8Residual residual = new Vp8Residual();
		do
		{
			bool flag2 = !Proba.UseSkipProba;
			rd.Clear();
			vp8EncIterator.Import(span, span2, span3, num2, uvStride, width, height, importBoundarySamples: false);
			if (!Decimate(vp8EncIterator, ref rd, rdOptLevel) || flag2)
			{
				CodeResiduals(vp8EncIterator, rd, residual);
			}
			else
			{
				vp8EncIterator.ResetAfterSkip();
			}
			vp8EncIterator.SaveBoundary();
		}
		while (vp8EncIterator.Next());
		AdjustFilterStrength();
		int size = 0;
		bool flag3 = false;
		Span<byte> span5 = Span<byte>.Empty;
		IMemoryOwner<byte> memoryOwner = null;
		try
		{
			if (flag)
			{
				memoryOwner = AlphaEncoder.EncodeAlpha(region, configuration, memoryAllocator, skipMetadata, alphaCompression, out size);
				span5 = memoryOwner.GetSpan();
				if (size < num)
				{
					flag3 = true;
				}
			}
			bitWriter.Finish();
			long sizePosition = 0L;
			if (hasAnimation)
			{
				sizePosition = new WebpFrameData((uint)bounds.X, (uint)bounds.Y, (uint)bounds.Width, (uint)bounds.Height, frameMetadata.FrameDelay, frameMetadata.BlendMethod, frameMetadata.DisposalMethod).WriteHeaderTo(stream);
			}
			if (flag)
			{
				Span<byte> dataBytes = span5.Slice(0, size);
				bool alphaDataIsCompressed = alphaCompression && flag3;
				BitWriterBase.WriteAlphaChunk(stream, dataBytes, alphaDataIsCompressed);
			}
			bitWriter.WriteEncodedImageToStream(stream);
			if (hasAnimation)
			{
				RiffHelper.EndWriteChunk(stream, sizePosition);
			}
		}
		finally
		{
			memoryOwner?.Dispose();
		}
		return flag;
	}

	public void Dispose()
	{
		Y.Dispose();
		U.Dispose();
		V.Dispose();
	}

	private void StatLoop(int width, int height, int yStride, int uvStride)
	{
		bool num = method == WebpEncodingMethod.Level0 || method == WebpEncodingMethod.Level3;
		int num2 = entropyPasses;
		Vp8RdLevel rdOpt = ((method >= WebpEncodingMethod.Level3) ? Vp8RdLevel.RdOptBasic : Vp8RdLevel.RdOptNone);
		int num3 = Mbw * Mbh;
		PassStats passStats = new PassStats(0L, 0f, 0, 100, quality);
		Proba.ResetTokenStats();
		if (num)
		{
			num3 = ((method != WebpEncodingMethod.Level3) ? ((num3 > 200) ? (num3 >> 2) : 50) : ((num3 > 200) ? (num3 >> 1) : 100));
		}
		while (num2-- > 0)
		{
			bool flag = MathF.Abs(passStats.Dq) <= 0.4f || num2 == 0 || maxI4HeaderBits == 0;
			long num4 = OneStatPass(width, height, yStride, uvStride, rdOpt, num3, passStats);
			if (num4 == 0L)
			{
				return;
			}
			if (maxI4HeaderBits > 0 && num4 > 1069547520)
			{
				num2++;
				maxI4HeaderBits >>= 1;
			}
			else if (flag)
			{
				break;
			}
		}
		Proba.FinalizeSkipProba(Mbw, Mbh);
		Proba.FinalizeTokenProbas();
		Proba.CalculateLevelCosts();
	}

	private long OneStatPass(int width, int height, int yStride, int uvStride, Vp8RdLevel rdOpt, int nbMbs, PassStats stats)
	{
		Span<byte> span = Y.GetSpan();
		Span<byte> span2 = U.GetSpan();
		Span<byte> span3 = V.GetSpan();
		Vp8EncIterator vp8EncIterator = new Vp8EncIterator(this);
		long num = 0L;
		long num2 = 0L;
		long num3 = 0L;
		long size = nbMbs * 384;
		vp8EncIterator.Init();
		SetLoopParams(stats.Q);
		Vp8ModeScore rd = new Vp8ModeScore();
		do
		{
			rd.Clear();
			vp8EncIterator.Import(span, span2, span3, yStride, uvStride, width, height, importBoundarySamples: false);
			if (Decimate(vp8EncIterator, ref rd, rdOpt))
			{
				Vp8EncProba proba = Proba;
				int nbSkip = proba.NbSkip + 1;
				proba.NbSkip = nbSkip;
			}
			RecordResiduals(vp8EncIterator, rd);
			num += rd.R + rd.H;
			num2 += rd.H;
			num3 += rd.D;
			vp8EncIterator.SaveBoundary();
		}
		while (vp8EncIterator.Next() && --nbMbs > 0);
		num2 += SegmentHeader.Size;
		if (stats.DoSizeSearch)
		{
			num += Proba.FinalizeSkipProba(Mbw, Mbh);
			num += Proba.FinalizeTokenProbas();
			num = (num + num2 + 1024 >> 11) + 30;
			stats.Value = num;
		}
		else
		{
			stats.Value = GetPsnr(num3, size);
		}
		return num2;
	}

	private void SetLoopParams(float q)
	{
		SetSegmentParams(q);
		SetSegmentProbas();
		ResetStats();
	}

	private unsafe void AdjustFilterStrength()
	{
		if (filterStrength <= 0)
		{
			return;
		}
		int num = 0;
		for (int i = 0; i < 4; i++)
		{
			Vp8SegmentInfo vp8SegmentInfo = SegmentInfos[i];
			int delta = vp8SegmentInfo.MaxEdge * vp8SegmentInfo.Y2.Q[1] >> 3;
			int num2 = FilterStrengthFromDelta(FilterHeader.Sharpness, delta);
			if (num2 > vp8SegmentInfo.FStrength)
			{
				vp8SegmentInfo.FStrength = num2;
			}
			if (num < vp8SegmentInfo.FStrength)
			{
				num = vp8SegmentInfo.FStrength;
			}
		}
		FilterHeader.FilterLevel = num;
	}

	private void ResetBoundaryPredictions()
	{
		Span<byte> span = Preds.AsSpan();
		Span<byte> span2 = Preds.AsSpan(PredsWidth - 1);
		for (int i = 0; i < 4 * Mbw; i++)
		{
			span[i] = 0;
		}
		for (int j = 0; j < 4 * Mbh; j++)
		{
			span2[j * PredsWidth] = 0;
		}
		int num = 4 * Mbw + 1;
		int num2 = 4 * Mbh + 1;
		int num3 = num * num2;
		Preds.AsSpan(num3 + PredsWidth - 4, 4).Clear();
		Nz[0] = 0u;
	}

	private void AssignSegments(ReadOnlySpan<int> alphas)
	{
		int num = ((SegmentHeader.NumSegments < 4) ? SegmentHeader.NumSegments : 4);
		Span<int> span = stackalloc int[4];
		int mid = 0;
		Span<int> span2 = stackalloc int[256];
		Span<int> span3 = stackalloc int[4];
		Span<int> span4 = stackalloc int[4];
		int i;
		for (i = 0; i <= 255 && alphas[i] == 0; i++)
		{
		}
		int num2 = i;
		i = 255;
		while (i > num2 && alphas[i] == 0)
		{
			i--;
		}
		int num3 = i;
		int num4 = num3 - num2;
		int num5 = 0;
		i = 1;
		while (num5 < num)
		{
			span[num5] = num2 + i * num4 / (2 * num);
			num5++;
			i += 2;
		}
		for (num5 = 0; num5 < 6; num5++)
		{
			for (i = 0; i < num; i++)
			{
				span3[i] = 0;
				span4[i] = 0;
			}
			i = 0;
			for (int j = num2; j <= num3; j++)
			{
				if (alphas[j] != 0)
				{
					for (; i + 1 < num && Math.Abs(j - span[i + 1]) < Math.Abs(j - span[i]); i++)
					{
					}
					span2[j] = i;
					span4[i] += j * alphas[j];
					span3[i] += alphas[j];
				}
			}
			int num6 = 0;
			mid = 0;
			int num7 = 0;
			for (i = 0; i < num; i++)
			{
				if (span3[i] != 0)
				{
					int num8 = (span4[i] + (span3[i] >> 1)) / span3[i];
					num6 += Math.Abs(span[i] - num8);
					span[i] = num8;
					mid += num8 * span3[i];
					num7 += span3[i];
				}
			}
			mid = (mid + (num7 >> 1)) / num7;
			if (num6 < 5)
			{
				break;
			}
		}
		for (i = 0; i < Mbw * Mbh; i++)
		{
			Vp8MacroBlockInfo obj = MbInfo[i];
			int index = obj.Alpha;
			obj.Segment = span2[index];
			obj.Alpha = span[span2[index]];
		}
		SetSegmentAlphas(span, mid);
	}

	private void SetSegmentAlphas(ReadOnlySpan<int> centers, int mid)
	{
		int numSegments = SegmentHeader.NumSegments;
		Vp8SegmentInfo[] segmentInfos = SegmentInfos;
		int num = centers[0];
		int num2 = centers[0];
		if (numSegments > 1)
		{
			for (int i = 0; i < numSegments; i++)
			{
				if (num > centers[i])
				{
					num = centers[i];
				}
				if (num2 < centers[i])
				{
					num2 = centers[i];
				}
			}
		}
		if (num2 == num)
		{
			num2 = num + 1;
		}
		for (int i = 0; i < numSegments; i++)
		{
			int value = 255 * (centers[i] - mid) / (num2 - num);
			int value2 = 255 * (centers[i] - num) / (num2 - num);
			segmentInfos[i].Alpha = Numerics.Clamp(value, -127, 127);
			segmentInfos[i].Beta = Numerics.Clamp(value2, 0, 255);
		}
	}

	private void SetSegmentParams(float quality)
	{
		int numSegments = SegmentHeader.NumSegments;
		Vp8SegmentInfo[] segmentInfos = SegmentInfos;
		double num = 0.9 * (double)spatialNoiseShaping / 100.0 / 128.0;
		double x = QualityToCompression((double)quality / 100.0);
		for (int i = 0; i < numSegments; i++)
		{
			double y = 1.0 - num * (double)segmentInfos[i].Alpha;
			double num2 = Math.Pow(x, y);
			int value = (int)(127.0 * (1.0 - num2));
			segmentInfos[i].Quant = Numerics.Clamp(value, 0, 127);
		}
		BaseQuant = segmentInfos[0].Quant;
		DqUvAc = (uvAlpha - 64) * 10 / 70;
		DqUvAc = DqUvAc * spatialNoiseShaping / 100;
		DqUvAc = Numerics.Clamp(DqUvAc, -4, 6);
		DqUvDc = -4 * spatialNoiseShaping / 100;
		DqUvDc = Numerics.Clamp(DqUvDc, -15, 15);
		DqY1Dc = 0;
		DqY2Dc = 0;
		DqY2Ac = 0;
		SetupFilterStrength();
		SetupMatrices(segmentInfos);
	}

	private void SetupFilterStrength()
	{
		int num = 5 * filterStrength;
		for (int i = 0; i < 4; i++)
		{
			Vp8SegmentInfo vp8SegmentInfo = SegmentInfos[i];
			int delta = WebpLookupTables.AcTable[Numerics.Clamp(vp8SegmentInfo.Quant, 0, 127)] >> 2;
			int num2 = FilterStrengthFromDelta(FilterHeader.Sharpness, delta) * num / (256 + vp8SegmentInfo.Beta);
			if (num2 < 2)
			{
				vp8SegmentInfo.FStrength = 0;
			}
			else if (num2 > 63)
			{
				vp8SegmentInfo.FStrength = 63;
			}
			else
			{
				vp8SegmentInfo.FStrength = num2;
			}
		}
		FilterHeader.FilterLevel = SegmentInfos[0].FStrength;
		FilterHeader.Simple = false;
		FilterHeader.Sharpness = 0;
	}

	private void SetSegmentProbas()
	{
		Span<int> span = stackalloc int[4];
		for (int i = 0; i < Mbw * Mbh; i++)
		{
			Vp8MacroBlockInfo vp8MacroBlockInfo = MbInfo[i];
			span[vp8MacroBlockInfo.Segment]++;
		}
		if (SegmentHeader.NumSegments > 1)
		{
			byte[] segments = Proba.Segments;
			segments[0] = (byte)GetProba(span[0] + span[1], span[2] + span[3]);
			segments[1] = (byte)GetProba(span[0], span[1]);
			segments[2] = (byte)GetProba(span[2], span[3]);
			SegmentHeader.UpdateMap = segments[0] != byte.MaxValue || segments[1] != byte.MaxValue || segments[2] != byte.MaxValue;
			if (!SegmentHeader.UpdateMap)
			{
				ResetSegments();
			}
			SegmentHeader.Size = span[0] * (LossyUtils.Vp8BitCost(0, segments[0]) + LossyUtils.Vp8BitCost(0, segments[1])) + span[1] * (LossyUtils.Vp8BitCost(0, segments[0]) + LossyUtils.Vp8BitCost(1, segments[1])) + span[2] * (LossyUtils.Vp8BitCost(1, segments[0]) + LossyUtils.Vp8BitCost(0, segments[2])) + span[3] * (LossyUtils.Vp8BitCost(1, segments[0]) + LossyUtils.Vp8BitCost(1, segments[2]));
		}
		else
		{
			SegmentHeader.UpdateMap = false;
			SegmentHeader.Size = 0;
		}
	}

	private void ResetSegments()
	{
		for (int i = 0; i < Mbw * Mbh; i++)
		{
			MbInfo[i].Segment = 0;
		}
	}

	private void ResetStats()
	{
		Vp8EncProba proba = Proba;
		proba.CalculateLevelCosts();
		proba.NbSkip = 0;
	}

	private unsafe void SetupMatrices(Vp8SegmentInfo[] dqm)
	{
		int num = ((method >= WebpEncodingMethod.Level4) ? spatialNoiseShaping : 0);
		foreach (Vp8SegmentInfo vp8SegmentInfo in dqm)
		{
			int quant = vp8SegmentInfo.Quant;
			ref ushort q = ref vp8SegmentInfo.Y1.Q[0];
			q = WebpLookupTables.DcTable[Numerics.Clamp(quant + DqY1Dc, 0, 127)];
			vp8SegmentInfo.Y1.Q[1] = WebpLookupTables.AcTable[Numerics.Clamp(quant, 0, 127)];
			ref ushort q2 = ref vp8SegmentInfo.Y2.Q[0];
			q2 = (ushort)(WebpLookupTables.DcTable[Numerics.Clamp(quant + DqY2Dc, 0, 127)] * 2);
			vp8SegmentInfo.Y2.Q[1] = WebpLookupTables.AcTable2[Numerics.Clamp(quant + DqY2Ac, 0, 127)];
			ref ushort q3 = ref vp8SegmentInfo.Uv.Q[0];
			q3 = WebpLookupTables.DcTable[Numerics.Clamp(quant + DqUvDc, 0, 117)];
			vp8SegmentInfo.Uv.Q[1] = WebpLookupTables.AcTable[Numerics.Clamp(quant + DqUvAc, 0, 127)];
			int num2 = vp8SegmentInfo.Y1.Expand(0);
			int num3 = vp8SegmentInfo.Y2.Expand(1);
			int num4 = vp8SegmentInfo.Uv.Expand(2);
			vp8SegmentInfo.LambdaI16 = 3 * num3 * num3;
			vp8SegmentInfo.LambdaI4 = 3 * num2 * num2 >> 7;
			vp8SegmentInfo.LambdaUv = 3 * num4 * num4 >> 6;
			vp8SegmentInfo.LambdaMode = num2 * num2 >> 7;
			vp8SegmentInfo.TLambda = num * num2 >> 5;
			vp8SegmentInfo.LambdaI16 = ((vp8SegmentInfo.LambdaI16 < 1) ? 1 : vp8SegmentInfo.LambdaI16);
			vp8SegmentInfo.LambdaI4 = ((vp8SegmentInfo.LambdaI4 < 1) ? 1 : vp8SegmentInfo.LambdaI4);
			vp8SegmentInfo.LambdaUv = ((vp8SegmentInfo.LambdaUv < 1) ? 1 : vp8SegmentInfo.LambdaUv);
			vp8SegmentInfo.LambdaMode = ((vp8SegmentInfo.LambdaMode < 1) ? 1 : vp8SegmentInfo.LambdaMode);
			vp8SegmentInfo.TLambda = ((vp8SegmentInfo.TLambda < 1) ? 1 : vp8SegmentInfo.TLambda);
			vp8SegmentInfo.MinDisto = 20 * vp8SegmentInfo.Y1.Q[0];
			vp8SegmentInfo.MaxEdge = 0;
			vp8SegmentInfo.I4Penalty = 1000 * num2 * num2;
		}
	}

	private int MacroBlockAnalysis(int width, int height, Vp8EncIterator it, Span<byte> y, Span<byte> u, Span<byte> v, int yStride, int uvStride, Span<int> alphas, out int uvAlpha)
	{
		int num = 0;
		uvAlpha = 0;
		if (!it.IsDone())
		{
			do
			{
				it.Import(y, u, v, yStride, uvStride, width, height, importBoundarySamples: true);
				int bestUvAlpha;
				int num2 = MbAnalyze(it, alphas, out bestUvAlpha);
				num += num2;
				uvAlpha += bestUvAlpha;
			}
			while (it.Next());
		}
		return num;
	}

	private int MbAnalyze(Vp8EncIterator it, Span<int> alphas, out int bestUvAlpha)
	{
		it.SetIntra16Mode(0);
		it.SetSkip(skip: false);
		it.SetSegment(0);
		int num;
		if (method <= WebpEncodingMethod.Level1)
		{
			num = it.FastMbAnalyze(quality);
		}
		else
		{
			num = it.MbAnalyzeBestIntra16Mode();
			if (method >= WebpEncodingMethod.Level5)
			{
				num = it.MbAnalyzeBestIntra4Mode(num);
			}
		}
		bestUvAlpha = it.MbAnalyzeBestUvMode();
		num = 3 * num + bestUvAlpha + 2 >> 2;
		num = FinalAlphaValue(num);
		alphas[num]++;
		it.CurrentMacroBlockInfo.Alpha = num;
		return num;
	}

	private bool Decimate(Vp8EncIterator it, ref Vp8ModeScore rd, Vp8RdLevel rdOpt)
	{
		rd.InitScore();
		it.MakeLuma16Preds();
		it.MakeChroma8Preds();
		if (rdOpt > Vp8RdLevel.RdOptNone)
		{
			QuantEnc.PickBestIntra16(it, ref rd, SegmentInfos, Proba);
			if (method >= WebpEncodingMethod.Level2)
			{
				QuantEnc.PickBestIntra4(it, ref rd, SegmentInfos, Proba, maxI4HeaderBits);
			}
			QuantEnc.PickBestUv(it, ref rd, SegmentInfos, Proba);
		}
		else
		{
			QuantEnc.RefineUsingDistortion(it, SegmentInfos, rd, method >= WebpEncodingMethod.Level2, method >= WebpEncodingMethod.Level1, MbHeaderLimit);
		}
		bool flag = rd.Nz == 0;
		it.SetSkip(flag);
		return flag;
	}

	private void CodeResiduals(Vp8EncIterator it, Vp8ModeScore rd, Vp8Residual residual)
	{
		bool flag = it.CurrentMacroBlockInfo.MacroBlockType == Vp8MacroBlockType.I16X16;
		int segment = it.CurrentMacroBlockInfo.Segment;
		it.NzToBytes();
		int numBytes = bitWriter.NumBytes;
		if (flag)
		{
			residual.Init(0, 1, Proba);
			residual.SetCoeffs(rd.YDcLevels);
			int num = bitWriter.PutCoeffs(it.TopNz[8] + it.LeftNz[8], residual);
			it.TopNz[8] = (it.LeftNz[8] = num);
			residual.Init(1, 0, Proba);
		}
		else
		{
			residual.Init(0, 3, Proba);
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int ctx = it.TopNz[j] + it.LeftNz[i];
				Span<short> coeffs = rd.YAcLevels.AsSpan(16 * (j + i * 4), 16);
				residual.SetCoeffs(coeffs);
				int num2 = bitWriter.PutCoeffs(ctx, residual);
				it.TopNz[j] = (it.LeftNz[i] = num2);
			}
		}
		int numBytes2 = bitWriter.NumBytes;
		residual.Init(0, 2, Proba);
		for (int k = 0; k <= 2; k += 2)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int ctx2 = it.TopNz[4 + k + j] + it.LeftNz[4 + k + i];
					residual.SetCoeffs(rd.UvLevels.AsSpan(16 * (k * 2 + j + i * 2), 16));
					int num3 = bitWriter.PutCoeffs(ctx2, residual);
					it.TopNz[4 + k + j] = (it.LeftNz[4 + k + i] = num3);
				}
			}
		}
		int numBytes3 = bitWriter.NumBytes;
		it.LumaBits = numBytes2 - numBytes;
		it.UvBits = numBytes3 - numBytes2;
		it.BitCount[segment, flag ? 1u : 0u] += it.LumaBits;
		it.BitCount[segment, 2] += it.UvBits;
		it.BytesToNz();
	}

	private void RecordResiduals(Vp8EncIterator it, Vp8ModeScore rd)
	{
		Vp8Residual vp8Residual = new Vp8Residual();
		bool num = it.CurrentMacroBlockInfo.MacroBlockType == Vp8MacroBlockType.I16X16;
		it.NzToBytes();
		if (num)
		{
			vp8Residual.Init(0, 1, Proba);
			vp8Residual.SetCoeffs(rd.YDcLevels);
			int num2 = vp8Residual.RecordCoeffs(it.TopNz[8] + it.LeftNz[8]);
			it.TopNz[8] = num2;
			it.LeftNz[8] = num2;
			vp8Residual.Init(1, 0, Proba);
		}
		else
		{
			vp8Residual.Init(0, 3, Proba);
		}
		for (int i = 0; i < 4; i++)
		{
			for (int j = 0; j < 4; j++)
			{
				int ctx = it.TopNz[j] + it.LeftNz[i];
				Span<short> coeffs = rd.YAcLevels.AsSpan(16 * (j + i * 4), 16);
				vp8Residual.SetCoeffs(coeffs);
				int num3 = vp8Residual.RecordCoeffs(ctx);
				it.TopNz[j] = num3;
				it.LeftNz[i] = num3;
			}
		}
		vp8Residual.Init(0, 2, Proba);
		for (int k = 0; k <= 2; k += 2)
		{
			for (int i = 0; i < 2; i++)
			{
				for (int j = 0; j < 2; j++)
				{
					int ctx2 = it.TopNz[4 + k + j] + it.LeftNz[4 + k + i];
					vp8Residual.SetCoeffs(rd.UvLevels.AsSpan(16 * (k * 2 + j + i * 2), 16));
					int num4 = vp8Residual.RecordCoeffs(ctx2);
					it.TopNz[4 + k + j] = num4;
					it.LeftNz[4 + k + i] = num4;
				}
			}
		}
		it.BytesToNz();
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int FinalAlphaValue(int alpha)
	{
		alpha = 255 - alpha;
		return Numerics.Clamp(alpha, 0, 255);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double QualityToCompression(double c)
	{
		return Math.Pow((c < 0.75) ? (c * (2.0 / 3.0)) : (2.0 * c - 1.0), 1.0 / 3.0);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int FilterStrengthFromDelta(int sharpness, int delta)
	{
		int num = ((delta < 64) ? delta : 63);
		return WebpLookupTables.LevelsFromDelta[sharpness, num];
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static double GetPsnr(long mse, long size)
	{
		if (mse <= 0 || size <= 0)
		{
			return 99.0;
		}
		return 10.0 * Math.Log10(65025f * (float)size / (float)mse);
	}

	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private static int GetProba(int a, int b)
	{
		int num = a + b;
		if (num != 0)
		{
			return (255 * a + (num >> 1)) / num;
		}
		return 255;
	}
}
