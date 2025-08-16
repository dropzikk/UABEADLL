using System;
using System.Buffers;
using System.Collections.Generic;
using SixLabors.ImageSharp.Memory;

namespace SixLabors.ImageSharp.Formats.Webp.Lossless;

internal sealed class CostManager : IDisposable
{
	private CostInterval? head;

	private const int FreeIntervalsStartCount = 25;

	private readonly Stack<CostInterval> freeIntervals = new Stack<CostInterval>(25);

	public int Count { get; set; }

	public List<double> CostCache { get; }

	public int CacheIntervalsSize { get; }

	public IMemoryOwner<float> Costs { get; }

	public IMemoryOwner<ushort> DistArray { get; }

	public List<CostCacheInterval> CacheIntervals { get; }

	public CostManager(MemoryAllocator memoryAllocator, IMemoryOwner<ushort> distArray, int pixCount, CostModel costModel)
	{
		int num = ((pixCount > 4095) ? 4095 : pixCount);
		CacheIntervals = new List<CostCacheInterval>();
		CostCache = new List<double>();
		Costs = memoryAllocator.Allocate<float>(pixCount);
		DistArray = distArray;
		Count = 0;
		for (int i = 0; i < 25; i++)
		{
			freeIntervals.Push(new CostInterval());
		}
		int cacheIntervalsSize = CacheIntervalsSize;
		CacheIntervalsSize = cacheIntervalsSize + 1;
		CostCache.Add(costModel.GetLengthCost(0));
		for (int j = 1; j < num; j++)
		{
			CostCache.Add(costModel.GetLengthCost(j));
			if (CostCache[j] != CostCache[j - 1])
			{
				cacheIntervalsSize = CacheIntervalsSize;
				CacheIntervalsSize = cacheIntervalsSize + 1;
			}
		}
		CostCacheInterval costCacheInterval = new CostCacheInterval
		{
			Start = 0,
			End = 1,
			Cost = CostCache[0]
		};
		CacheIntervals.Add(costCacheInterval);
		for (int k = 1; k < num; k++)
		{
			double num2 = CostCache[k];
			if (num2 != costCacheInterval.Cost)
			{
				costCacheInterval = new CostCacheInterval
				{
					Start = k,
					Cost = num2
				};
				CacheIntervals.Add(costCacheInterval);
			}
			costCacheInterval.End = k + 1;
		}
		Costs.GetSpan().Fill(1E+38f);
	}

	public void UpdateCostAtIndex(int i, bool doCleanIntervals)
	{
		CostInterval costInterval = head;
		while (costInterval != null && costInterval.Start <= i)
		{
			CostInterval? next = costInterval.Next;
			if (costInterval.End <= i)
			{
				if (doCleanIntervals)
				{
					PopInterval(costInterval);
				}
			}
			else
			{
				UpdateCost(i, costInterval.Index, costInterval.Cost);
			}
			costInterval = next;
		}
	}

	public void PushInterval(double distanceCost, int position, int len)
	{
		int num = 10;
		Span<float> span = Costs.GetSpan();
		Span<ushort> span2 = DistArray.GetSpan();
		if (len < num)
		{
			for (int i = position; i < position + len; i++)
			{
				int num2 = i - position;
				float num3 = (float)(distanceCost + CostCache[num2]);
				if (span[i] > num3)
				{
					span[i] = num3;
					span2[i] = (ushort)(num2 + 1);
				}
			}
			return;
		}
		CostInterval costInterval = head;
		for (int j = 0; j < CacheIntervalsSize && CacheIntervals[j].Start < len; j++)
		{
			int num4 = position + CacheIntervals[j].Start;
			int num5 = position + ((CacheIntervals[j].End > len) ? len : CacheIntervals[j].End);
			float num6 = (float)(distanceCost + CacheIntervals[j].Cost);
			while (costInterval != null && costInterval.Start < num5)
			{
				CostInterval next = costInterval.Next;
				if (num4 < costInterval.End)
				{
					if (num6 >= costInterval.Cost)
					{
						int end = costInterval.End;
						InsertInterval(costInterval, num6, position, num4, costInterval.Start);
						num4 = end;
						if (num4 >= num5)
						{
							break;
						}
					}
					else if (num4 <= costInterval.Start)
					{
						if (costInterval.End > num5)
						{
							costInterval.Start = num5;
							break;
						}
						PopInterval(costInterval);
					}
					else
					{
						if (num5 < costInterval.End)
						{
							int end2 = costInterval.End;
							costInterval.End = num4;
							InsertInterval(costInterval, costInterval.Cost, costInterval.Index, num5, end2);
							break;
						}
						costInterval.End = num4;
					}
				}
				costInterval = next;
			}
			InsertInterval(costInterval, num6, position, num4, num5);
		}
	}

	private void PopInterval(CostInterval? interval)
	{
		if (interval != null)
		{
			ConnectIntervals(interval.Previous, interval.Next);
			Count--;
			interval.Next = null;
			interval.Previous = null;
			freeIntervals.Push(interval);
		}
	}

	private void InsertInterval(CostInterval? intervalIn, float cost, int position, int start, int end)
	{
		if (start < end)
		{
			CostInterval costInterval;
			if (freeIntervals.Count > 0)
			{
				costInterval = freeIntervals.Pop();
				costInterval.Cost = cost;
				costInterval.Start = start;
				costInterval.End = end;
				costInterval.Index = position;
			}
			else
			{
				costInterval = new CostInterval
				{
					Cost = cost,
					Start = start,
					End = end,
					Index = position
				};
			}
			PositionOrphanInterval(costInterval, intervalIn);
			Count++;
		}
	}

	private void PositionOrphanInterval(CostInterval current, CostInterval? previous)
	{
		if (previous == null)
		{
			previous = head;
		}
		while (previous != null && current.Start < previous.Start)
		{
			previous = previous.Previous;
		}
		while (previous?.Next != null && previous.Next.Start < current.Start)
		{
			previous = previous.Next;
		}
		ConnectIntervals(current, (previous != null) ? previous.Next : head);
		ConnectIntervals(previous, current);
	}

	private void ConnectIntervals(CostInterval? prev, CostInterval? next)
	{
		if (prev != null)
		{
			prev.Next = next;
		}
		else
		{
			head = next;
		}
		if (next != null)
		{
			next.Previous = prev;
		}
	}

	private void UpdateCost(int i, int position, float cost)
	{
		Span<float> span = Costs.GetSpan();
		Span<ushort> span2 = DistArray.GetSpan();
		int num = i - position;
		if (span[i] > cost)
		{
			span[i] = cost;
			span2[i] = (ushort)(num + 1);
		}
	}

	public void Dispose()
	{
		Costs.Dispose();
	}
}
