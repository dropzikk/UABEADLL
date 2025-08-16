using System;

namespace Avalonia.Media;

internal static class PreciseEllipticArcHelper
{
	public sealed class EllipticalArc
	{
		private readonly struct SimpleMatrix
		{
			private readonly double _a;

			private readonly double _b;

			private readonly double _c;

			private readonly double _d;

			public SimpleMatrix(double a, double b, double c, double d)
			{
				_a = a;
				_b = b;
				_c = c;
				_d = d;
			}

			public static Point operator *(SimpleMatrix m, Point p)
			{
				return new Point(m._a * p.X + m._b * p.Y, m._c * p.X + m._d * p.Y);
			}
		}

		private const double TwoPi = Math.PI * 2.0;

		private static readonly double[][][] Coeffs2Low = new double[2][][]
		{
			new double[4][]
			{
				new double[4] { 3.92478, -13.5822, -0.233377, 0.0128206 },
				new double[4] { -1.08814, 0.859987, 0.000362265, 0.000229036 },
				new double[4] { -0.942512, 0.390456, 0.0080909, 0.00723895 },
				new double[4] { -0.736228, 0.20998, 0.0129867, 0.0103456 }
			},
			new double[4][]
			{
				new double[4] { -0.395018, 6.82464, 0.0995293, 0.0122198 },
				new double[4] { -0.545608, 0.0774863, 0.0267327, 0.0132482 },
				new double[4] { 0.0534754, -0.0884167, 0.012595, 0.0343396 },
				new double[4] { 0.209052, -0.0599987, -0.00723897, 0.00789976 }
			}
		};

		private static readonly double[][][] Coeffs2High = new double[2][][]
		{
			new double[4][]
			{
				new double[4] { 0.0863805, -11.5595, -2.68765, 0.181224 },
				new double[4] { 0.242856, -1.81073, 1.56876, 1.68544 },
				new double[4] { 0.233337, -0.455621, 0.222856, 0.403469 },
				new double[4] { 0.0612978, -0.104879, 0.0446799, 0.00867312 }
			},
			new double[4][]
			{
				new double[4] { 0.028973, 6.68407, 0.171472, 0.0211706 },
				new double[4] { 0.0307674, -0.0517815, 0.0216803, -0.0749348 },
				new double[4] { -0.0471179, 0.1288, -0.0781702, 2.0 },
				new double[4] { -0.0309683, 0.0531557, -0.0227191, 0.0434511 }
			}
		};

		private static readonly double[] Safety2 = new double[4] { 0.02, 2.83, 0.125, 0.01 };

		private static readonly double[][][] Coeffs3Low = new double[2][][]
		{
			new double[4][]
			{
				new double[4] { 3.85268, -21.229, -0.330434, 0.0127842 },
				new double[4] { -1.61486, 0.706564, 0.225945, 0.263682 },
				new double[4] { -0.910164, 0.388383, 0.00551445, 0.00671814 },
				new double[4] { -0.630184, 0.192402, 0.0098871, 0.0102527 }
			},
			new double[4][]
			{
				new double[4] { -0.162211, 9.94329, 0.13723, 0.0124084 },
				new double[4] { -0.253135, 0.00187735, 0.0230286, 0.01264 },
				new double[4] { -0.0695069, -0.0437594, 0.0120636, 0.0163087 },
				new double[4] { -0.0328856, -0.00926032, -0.00173573, 0.00527385 }
			}
		};

		private static readonly double[][][] Coeffs3High = new double[2][][]
		{
			new double[4][]
			{
				new double[4] { 0.0899116, -19.2349, -4.11711, 0.183362 },
				new double[4] { 0.138148, -1.45804, 1.32044, 1.38474 },
				new double[4] { 0.230903, -0.450262, 0.219963, 0.414038 },
				new double[4] { 0.0590565, -0.101062, 0.0430592, 0.0204699 }
			},
			new double[4][]
			{
				new double[4] { 0.0164649, 9.89394, 0.0919496, 0.00760802 },
				new double[4] { 0.0191603, -0.0322058, 0.0134667, -0.0825018 },
				new double[4] { 0.0156192, -0.017535, 0.00326508, -0.228157 },
				new double[4] { -0.0236752, 0.0405821, -0.0173086, 0.176187 }
			}
		};

		private static readonly double[] Safety3 = new double[4] { 0.001, 4.98, 0.207, 0.0067 };

		internal double Cx;

		internal double Cy;

		internal double A;

		internal double B;

		internal double Theta;

		private readonly double _cosTheta;

		private readonly double _sinTheta;

		internal double Eta1;

		internal double Eta2;

		internal double X1;

		internal double Y1;

		internal double X2;

		internal double Y2;

		internal double FirstFocusX;

		internal double FirstFocusY;

		internal double SecondFocusX;

		internal double SecondFocusY;

		private double _xLeft;

		private double _yUp;

		private double _width;

		private double _height;

		internal bool IsPieSlice;

		private int _maxDegree;

		private double _defaultFlatness;

		internal double F;

		internal double E2;

		internal double G;

		internal double G2;

		public bool DrawInOppositeDirection { get; set; }

		public EllipticalArc()
		{
			Cx = 0.0;
			Cy = 0.0;
			A = 1.0;
			B = 1.0;
			Theta = 0.0;
			Eta1 = 0.0;
			Eta2 = Math.PI * 2.0;
			_cosTheta = 1.0;
			_sinTheta = 0.0;
			IsPieSlice = false;
			_maxDegree = 3;
			_defaultFlatness = 0.5;
			ComputeFocii();
			ComputeEndPoints();
			ComputeBounds();
			ComputeDerivedFlatnessParameters();
		}

		public EllipticalArc(Point center, double a, double b, double theta, double lambda1, double lambda2, bool isPieSlice)
			: this(center.X, center.Y, a, b, theta, lambda1, lambda2, isPieSlice)
		{
		}

		public EllipticalArc(double cx, double cy, double a, double b, double theta, double lambda1, double lambda2, bool isPieSlice)
		{
			Cx = cx;
			Cy = cy;
			A = a;
			B = b;
			Theta = theta;
			IsPieSlice = isPieSlice;
			Eta1 = Math.Atan2(Math.Sin(lambda1) / b, Math.Cos(lambda1) / a);
			Eta2 = Math.Atan2(Math.Sin(lambda2) / b, Math.Cos(lambda2) / a);
			_cosTheta = Math.Cos(theta);
			_sinTheta = Math.Sin(theta);
			_maxDegree = 3;
			_defaultFlatness = 0.5;
			Eta2 -= Math.PI * 2.0 * Math.Floor((Eta2 - Eta1) / (Math.PI * 2.0));
			if (lambda2 - lambda1 > Math.PI && Eta2 - Eta1 < Math.PI)
			{
				Eta2 += Math.PI * 2.0;
			}
			ComputeFocii();
			ComputeEndPoints();
			ComputeBounds();
			ComputeDerivedFlatnessParameters();
		}

		public EllipticalArc(Point center, double a, double b, double theta)
			: this(center.X, center.Y, a, b, theta)
		{
		}

		public EllipticalArc(double cx, double cy, double a, double b, double theta)
		{
			Cx = cx;
			Cy = cy;
			A = a;
			B = b;
			Theta = theta;
			IsPieSlice = false;
			Eta1 = 0.0;
			Eta2 = Math.PI * 2.0;
			_cosTheta = Math.Cos(theta);
			_sinTheta = Math.Sin(theta);
			_maxDegree = 3;
			_defaultFlatness = 0.5;
			ComputeFocii();
			ComputeEndPoints();
			ComputeBounds();
			ComputeDerivedFlatnessParameters();
		}

		public void SetMaxDegree(int maxDegree)
		{
			if (maxDegree < 1 || maxDegree > 3)
			{
				throw new ArgumentException("maxDegree must be between 1 and 3", "maxDegree");
			}
			_maxDegree = maxDegree;
		}

		public void SetDefaultFlatness(double defaultFlatness)
		{
			if (defaultFlatness < 1E-10)
			{
				throw new ArgumentException("defaultFlatness must be greater than 1.0e-10", "defaultFlatness");
			}
			_defaultFlatness = defaultFlatness;
		}

		private void ComputeFocii()
		{
			double num = Math.Sqrt(A * A - B * B);
			double num2 = num * _cosTheta;
			double num3 = num * _sinTheta;
			FirstFocusX = Cx - num2;
			FirstFocusY = Cy - num3;
			SecondFocusX = Cx + num2;
			SecondFocusY = Cy + num3;
		}

		private void ComputeEndPoints()
		{
			double num = A * Math.Cos(Eta1);
			double num2 = B * Math.Sin(Eta1);
			X1 = Cx + num * _cosTheta - num2 * _sinTheta;
			Y1 = Cy + num * _sinTheta + num2 * _cosTheta;
			double num3 = A * Math.Cos(Eta2);
			double num4 = B * Math.Sin(Eta2);
			X2 = Cx + num3 * _cosTheta - num4 * _sinTheta;
			Y2 = Cy + num3 * _sinTheta + num4 * _cosTheta;
		}

		private void ComputeBounds()
		{
			double num = B / A;
			double num3;
			double num5;
			double num4;
			double num6;
			if (Math.Abs(_sinTheta) < 0.1)
			{
				double num2 = _sinTheta / _cosTheta;
				if (_cosTheta < 0.0)
				{
					num3 = 0.0 - Math.Atan(num2 * num);
					num4 = num3 + Math.PI;
					num5 = Math.PI / 2.0 - Math.Atan(num2 / num);
					num6 = num5 + Math.PI;
				}
				else
				{
					num4 = 0.0 - Math.Atan(num2 * num);
					num3 = num4 - Math.PI;
					num6 = Math.PI / 2.0 - Math.Atan(num2 / num);
					num5 = num6 - Math.PI;
				}
			}
			else
			{
				double num7 = _cosTheta / _sinTheta;
				if (_sinTheta < 0.0)
				{
					num4 = Math.PI / 2.0 + Math.Atan(num7 / num);
					num3 = num4 - Math.PI;
					num5 = Math.Atan(num7 * num);
					num6 = num5 + Math.PI;
				}
				else
				{
					num3 = Math.PI / 2.0 + Math.Atan(num7 / num);
					num4 = num3 + Math.PI;
					num6 = Math.Atan(num7 * num);
					num5 = num6 - Math.PI;
				}
			}
			num3 -= Math.PI * 2.0 * Math.Floor((num3 - Eta1) / (Math.PI * 2.0));
			num5 -= Math.PI * 2.0 * Math.Floor((num5 - Eta1) / (Math.PI * 2.0));
			num4 -= Math.PI * 2.0 * Math.Floor((num4 - Eta1) / (Math.PI * 2.0));
			num6 -= Math.PI * 2.0 * Math.Floor((num6 - Eta1) / (Math.PI * 2.0));
			_xLeft = ((num3 <= Eta2) ? (Cx + A * Math.Cos(num3) * _cosTheta - B * Math.Sin(num3) * _sinTheta) : Math.Min(X1, X2));
			_yUp = ((num5 <= Eta2) ? (Cy + A * Math.Cos(num5) * _sinTheta + B * Math.Sin(num5) * _cosTheta) : Math.Min(Y1, Y2));
			_width = ((num4 <= Eta2) ? (Cx + A * Math.Cos(num4) * _cosTheta - B * Math.Sin(num4) * _sinTheta) : Math.Max(X1, X2)) - _xLeft;
			_height = ((num6 <= Eta2) ? (Cy + A * Math.Cos(num6) * _sinTheta + B * Math.Sin(num6) * _cosTheta) : Math.Max(Y1, Y2)) - _yUp;
		}

		private void ComputeDerivedFlatnessParameters()
		{
			F = (A - B) / A;
			E2 = F * (2.0 - F);
			G = 1.0 - F;
			G2 = G * G;
		}

		private static double RationalFunction(double x, double[] c)
		{
			return (x * (x * c[0] + c[1]) + c[2]) / (x + c[3]);
		}

		public double EstimateError(int degree, double etaA, double etaB)
		{
			if (degree < 1 || degree > _maxDegree)
			{
				throw new ArgumentException($"degree should be between {1} and {_maxDegree}", "degree");
			}
			double num = 0.5 * (etaA + etaB);
			if (degree < 2)
			{
				double num2 = A * Math.Cos(etaA);
				double num3 = B * Math.Sin(etaA);
				double num4 = Cx + num2 * _cosTheta - num3 * _sinTheta;
				double num5 = Cy + num2 * _sinTheta + num3 * _cosTheta;
				double num6 = A * Math.Cos(etaB);
				double num7 = B * Math.Sin(etaB);
				double num8 = Cx + num6 * _cosTheta - num7 * _sinTheta;
				double num9 = Cy + num6 * _sinTheta + num7 * _cosTheta;
				double num10 = A * Math.Cos(num);
				double num11 = B * Math.Sin(num);
				double num12 = Cx + num10 * _cosTheta - num11 * _sinTheta;
				double num13 = Cy + num10 * _sinTheta + num11 * _cosTheta;
				double num14 = num8 - num4;
				double num15 = num9 - num5;
				return Math.Abs(num12 * num15 - num13 * num14 + num8 * num5 - num4 * num9) / Math.Sqrt(num14 * num14 + num15 * num15);
			}
			double num16 = B / A;
			double num17 = etaB - etaA;
			double num18 = Math.Cos(2.0 * num);
			double num19 = Math.Cos(4.0 * num);
			double num20 = Math.Cos(6.0 * num);
			double[][][] array;
			double[] c;
			if (degree == 2)
			{
				array = ((num16 < 0.25) ? Coeffs2Low : Coeffs2High);
				c = Safety2;
			}
			else
			{
				array = ((num16 < 0.25) ? Coeffs3Low : Coeffs3High);
				c = Safety3;
			}
			double num21 = RationalFunction(num16, array[0][0]) + num18 * RationalFunction(num16, array[0][1]) + num19 * RationalFunction(num16, array[0][2]) + num20 * RationalFunction(num16, array[0][3]);
			double num22 = RationalFunction(num16, array[1][0]) + num18 * RationalFunction(num16, array[1][1]) + num19 * RationalFunction(num16, array[1][2]) + num20 * RationalFunction(num16, array[1][3]);
			return RationalFunction(num16, c) * A * Math.Exp(num21 + num22 * num17);
		}

		public Point PointAt(double lambda)
		{
			double num = Math.Atan2(Math.Sin(lambda) / B, Math.Cos(lambda) / A);
			double num2 = A * Math.Cos(num);
			double num3 = B * Math.Sin(num);
			return new Point(Cx + num2 * _cosTheta - num3 * _sinTheta, Cy + num2 * _sinTheta + num3 * _cosTheta);
		}

		public bool Contains(double x, double y)
		{
			double num = x - FirstFocusX;
			double num2 = y - FirstFocusY;
			double num3 = x - SecondFocusX;
			double num4 = y - SecondFocusY;
			if (num * num + num2 * num2 + num3 * num3 + num4 * num4 > 4.0 * A * A)
			{
				return false;
			}
			if (IsPieSlice)
			{
				double num5 = x - Cx;
				double num6 = y - Cy;
				double num7 = num5 * _cosTheta + num6 * _sinTheta;
				double num8 = Math.Atan2((num6 * _cosTheta - num5 * _sinTheta) / B, num7 / A);
				num8 -= Math.PI * 2.0 * Math.Floor((num8 - Eta1) / (Math.PI * 2.0));
				return num8 <= Eta2;
			}
			double num9 = X2 - X1;
			double num10 = Y2 - Y1;
			return x * num10 - y * num9 + X2 * Y1 - X1 * Y2 >= 0.0;
		}

		private bool IntersectArc(double xA, double yA, double xB, double yB)
		{
			double num = xA - xB;
			double num2 = yA - yB;
			double num3 = Math.Sqrt(num * num + num2 * num2);
			if (num3 < 1E-10 * A)
			{
				return false;
			}
			double num4 = (num * _cosTheta + num2 * _sinTheta) / num3;
			double num5 = (num2 * _cosTheta - num * _sinTheta) / num3;
			num = xA - Cx;
			num2 = yA - Cy;
			double num6 = num * _cosTheta + num2 * _sinTheta;
			double num7 = num2 * _cosTheta - num * _sinTheta;
			double num8 = num6 * num6;
			double num9 = num7 * num7;
			double num10 = G2 * (num8 - A * A) + num9;
			double num11 = 1.0 - E2 * num4 * num4;
			double num12 = G2 * num6 * num4 + num7 * num5;
			double num13 = num10;
			double num14 = num12 * num12;
			double num15 = num11 * num13;
			if (num14 < num15)
			{
				return false;
			}
			double num16 = ((num12 >= 0.0) ? ((num12 + Math.Sqrt(num14 - num15)) / num11) : (num13 / (num12 - Math.Sqrt(num14 - num15))));
			if (num16 >= 0.0 && num16 <= num3)
			{
				double num17 = num6 - num16 * num4;
				double num18 = Math.Atan2((num7 - num16 * num5) / B, num17 / A);
				num18 -= Math.PI * 2.0 * Math.Floor((num18 - Eta1) / (Math.PI * 2.0));
				if (num18 <= Eta2)
				{
					return true;
				}
			}
			num16 = num13 / (num16 * num11);
			if (num16 >= 0.0 && num16 <= num3)
			{
				double num19 = num6 - num16 * num4;
				double num20 = Math.Atan2((num7 - num16 * num5) / B, num19 / A);
				num20 -= Math.PI * 2.0 * Math.Floor((num20 - Eta1) / (Math.PI * 2.0));
				if (num20 <= Eta2)
				{
					return true;
				}
			}
			return false;
		}

		private static bool Intersect(double x1, double y1, double x2, double y2, double xA, double yA, double xB, double yB)
		{
			double num = x2 - x1;
			double num2 = y2 - y1;
			double num3 = x2 * y1 - x1 * y2;
			double num4 = xB - xA;
			double num5 = yB - yA;
			double num6 = xB * yA - xA * yB;
			double num7 = xA * num2 - yA * num + num3;
			double num8 = xB * num2 - yB * num + num3;
			double num9 = x1 * num5 - y1 * num4 + num6;
			double num10 = x2 * num5 - y2 * num4 + num6;
			if (num7 * num8 <= 0.0)
			{
				return num9 * num10 <= 0.0;
			}
			return false;
		}

		private bool IntersectOutline(double xA, double yA, double xB, double yB)
		{
			if (IntersectArc(xA, yA, xB, yB))
			{
				return true;
			}
			if (IsPieSlice)
			{
				if (!Intersect(Cx, Cy, X1, Y1, xA, yA, xB, yB))
				{
					return Intersect(Cx, Cy, X2, Y2, xA, yA, xB, yB);
				}
				return true;
			}
			return Intersect(X1, Y1, X2, Y2, xA, yA, xB, yB);
		}

		public bool Contains(double x, double y, double w, double h)
		{
			double num = x + w;
			double num2 = y + h;
			if (Contains(x, y) && Contains(num, y) && Contains(x, num2) && Contains(num, num2) && !IntersectOutline(x, y, num, y) && !IntersectOutline(num, y, num, num2) && !IntersectOutline(num, num2, x, num2))
			{
				return !IntersectOutline(x, num2, x, y);
			}
			return false;
		}

		public bool Contains(Point p)
		{
			return Contains(p.X, p.Y);
		}

		public bool Contains(Rect r)
		{
			return Contains(r.X, r.Y, r.Width, r.Height);
		}

		public Rect GetBounds()
		{
			return new Rect(_xLeft, _yUp, _width, _height);
		}

		public void BuildArc(StreamGeometryContext path)
		{
			BuildArc(path, _maxDegree, _defaultFlatness, openNewFigure: true);
		}

		public void BuildArc(StreamGeometryContext path, int degree, double threshold, bool openNewFigure)
		{
			if (degree < 1 || degree > _maxDegree)
			{
				throw new ArgumentException($"degree should be between {1} and {_maxDegree}", "degree");
			}
			bool flag = false;
			int num = 1;
			double num2;
			double num3;
			while (!flag && num < 1024)
			{
				num2 = (Eta2 - Eta1) / (double)num;
				if (num2 <= Math.PI / 2.0)
				{
					num3 = Eta1;
					flag = true;
					int num4 = 0;
					while (flag && num4 < num)
					{
						double etaA = num3;
						num3 += num2;
						flag = EstimateError(degree, etaA, num3) <= threshold;
						num4++;
					}
				}
				num <<= 1;
			}
			if (!DrawInOppositeDirection)
			{
				num2 = (Eta2 - Eta1) / (double)num;
				num3 = Eta1;
			}
			else
			{
				num2 = (Eta1 - Eta2) / (double)num;
				num3 = Eta2;
			}
			double num5 = Math.Cos(num3);
			double num6 = Math.Sin(num3);
			double num7 = A * num5;
			double num8 = B * num6;
			double num9 = A * num6;
			double num10 = B * num5;
			double num11 = Cx + num7 * _cosTheta - num8 * _sinTheta;
			double num12 = Cy + num7 * _sinTheta + num8 * _cosTheta;
			double num13 = (0.0 - num9) * _cosTheta - num10 * _sinTheta;
			double num14 = (0.0 - num9) * _sinTheta + num10 * _cosTheta;
			double num15 = Math.Tan(0.5 * num2);
			double num16 = Math.Sin(num2) * (Math.Sqrt(4.0 + 3.0 * num15 * num15) - 1.0) / 3.0;
			for (int i = 0; i < num; i++)
			{
				double num17 = num11;
				double num18 = num12;
				double num19 = num13;
				double num20 = num14;
				num3 += num2;
				num5 = Math.Cos(num3);
				num6 = Math.Sin(num3);
				num7 = A * num5;
				num8 = B * num6;
				double num21 = A * num6;
				num10 = B * num5;
				num11 = Cx + num7 * _cosTheta - num8 * _sinTheta;
				num12 = Cy + num7 * _sinTheta + num8 * _cosTheta;
				num13 = (0.0 - num21) * _cosTheta - num10 * _sinTheta;
				num14 = (0.0 - num21) * _sinTheta + num10 * _cosTheta;
				switch (degree)
				{
				case 1:
					path.LineTo(new Point(num11, num12));
					break;
				case 2:
				{
					double num22 = (num14 * (num11 - num17) - num13 * (num12 - num18)) / (num19 * num14 - num20 * num13);
					path.QuadraticBezierTo(new Point(num17 + num22 * num19, num18 + num22 * num20), new Point(num11, num12));
					break;
				}
				default:
					path.CubicBezierTo(new Point(num17 + num16 * num19, num18 + num16 * num20), new Point(num11 - num16 * num13, num12 - num16 * num14), new Point(num11, num12));
					break;
				}
			}
			if (IsPieSlice)
			{
				path.LineTo(new Point(Cx, Cy));
			}
		}

		private static double GetAngle(Vector v1, Vector v2)
		{
			double x = v1 * v2;
			return Math.Atan2(v1.X * v2.Y - v2.X * v1.Y, x);
		}

		public static void BuildArc(StreamGeometryContext path, Point p1, Point p2, Size size, double theta, bool isLargeArc, bool clockwise)
		{
			SimpleMatrix simpleMatrix = new SimpleMatrix(Math.Cos(theta), Math.Sin(theta), 0.0 - Math.Sin(theta), Math.Cos(theta));
			SimpleMatrix simpleMatrix2 = new SimpleMatrix(Math.Cos(theta), 0.0 - Math.Sin(theta), Math.Sin(theta), Math.Cos(theta));
			Point point = simpleMatrix * new Point((p1.X - p2.X) / 2.0, (p1.Y - p2.Y) / 2.0);
			double num = size.Width;
			double num2 = size.Height;
			double num3 = num * num;
			double num4 = num2 * num2;
			double num5 = point.Y * point.Y;
			double num6 = point.X * point.X;
			double num7 = num3 * num4 - num3 * num5 - num4 * num6;
			double num8 = num3 * num5 + num4 * num6;
			if (Math.Abs(num8) < 1E-08)
			{
				path.LineTo(p2);
				return;
			}
			if (num7 / num8 < 0.0)
			{
				double num9 = num6 / num3 + num5 / num4;
				double num10 = Math.Sqrt(num9);
				if (num9 > 1.0)
				{
					num *= num10;
					num2 *= num10;
					num3 = num * num;
					num4 = num2 * num2;
					num7 = num3 * num4 - num3 * num5 - num4 * num6;
					if (num7 < 0.0)
					{
						num7 = 0.0;
					}
					num8 = num3 * num5 + num4 * num6;
				}
			}
			double num11 = Math.Sqrt(Math.Abs(num7 / num8));
			Point point2 = new Point(num * point.Y / num2, (0.0 - num2) * point.X / num);
			int num12 = ((clockwise != isLargeArc) ? 1 : (-1));
			Point point3 = new Point(point2.X * num11 * (double)num12, point2.Y * num11 * (double)num12);
			Vector vector = new Vector((p1.X + p2.X) / 2.0, (p1.Y + p2.Y) / 2.0);
			Point point4 = simpleMatrix2 * point3 + vector;
			Point point5 = simpleMatrix * (p1 - point4);
			Point point6 = simpleMatrix * (p2 - point4);
			Point point7 = (clockwise ? point5 : point6);
			Point point8 = (clockwise ? point6 : point5);
			double angle = GetAngle(new Vector(1.0, 0.0), point7);
			double angle2 = GetAngle(new Vector(1.0, 0.0), point8);
			EllipticalArc ellipticalArc = new EllipticalArc(point4.X, point4.Y, num, num2, theta, angle, angle2, isPieSlice: false);
			if (ManhattanDistance(p2, new Point(ellipticalArc.X2, ellipticalArc.Y2)) > ManhattanDistance(p2, new Point(ellipticalArc.X1, ellipticalArc.Y1)))
			{
				ellipticalArc.DrawInOppositeDirection = true;
			}
			ellipticalArc.BuildArc(path, ellipticalArc._maxDegree, ellipticalArc._defaultFlatness, openNewFigure: false);
			static double ManhattanDistance(Point p1, Point p2)
			{
				return Math.Abs(p1.X - p2.X) + Math.Abs(p1.Y - p2.Y);
			}
		}

		public bool Intersects(double x, double y, double w, double h)
		{
			double num = x + w;
			double num2 = y + h;
			if (!Contains(x, y) && !Contains(num, y) && !Contains(x, num2) && !Contains(num, num2) && !IntersectOutline(x, y, num, y) && !IntersectOutline(num, y, num, num2) && !IntersectOutline(num, num2, x, num2))
			{
				return IntersectOutline(x, num2, x, y);
			}
			return true;
		}

		public bool Intersects(Rect r)
		{
			return Intersects(r.X, r.Y, r.Width, r.Height);
		}
	}

	public static void ArcTo(StreamGeometryContext streamGeometryContextImpl, Point currentPoint, Point point, Size size, double rotationAngle, bool isLargeArc, SweepDirection sweepDirection)
	{
		EllipticalArc.BuildArc(streamGeometryContextImpl, currentPoint, point, size, rotationAngle * (Math.PI / 180.0), isLargeArc, sweepDirection == SweepDirection.Clockwise);
	}
}
