using System;

namespace Guardian.Utilities
{
	internal class MathHelper
	{
		private static readonly Random random = new Random();

		public static int Abs(int val)
		{
			if (val >= 0)
			{
				return val;
			}
			return -val;
		}

		public static int Floor(float val)
		{
			int num = (int)val;
			if (!((float)num <= val))
			{
				return num - 1;
			}
			return num;
		}

		public static int Ceil(float val)
		{
			int num = (int)val;
			if (!((float)num >= val))
			{
				return num + 1;
			}
			return num;
		}

		public static int Clamp(int val, int min, int max)
		{
			if (val >= min)
			{
				if (val <= max)
				{
					return val;
				}
				return max;
			}
			return min;
		}

		public static int RandomInt(int min, int max)
		{
			if (min > max)
			{
				min += max;
				max = min - max;
				min -= max;
			}
			return random.Next(min, max);
		}

		public static int NextPowerOf2(int val)
		{
			int num = val - 1;
			int num2 = num | (num >> 1);
			int num3 = num2 | (num2 >> 2);
			int num4 = num3 | (num3 >> 4);
			int num5 = num4 | (num4 >> 8);
			return (num5 | (num5 >> 16)) + 1;
		}

		public static bool IsPowerOf2(int val)
		{
			return (val & (val - 1)) == 0;
		}
	}
}
