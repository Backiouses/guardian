using System.Collections.Generic;

namespace Guardian.AntiAbuse
{
	internal class ModifiedStats
	{
		public static readonly byte InfiniteGas = 1;

		public static readonly byte InfiniteBlades = 2;

		public static readonly byte InfiniteAhssAmmo = 4;

		public static List<char> FromInt(int val)
		{
			List<char> list = new List<char>();
			if ((val & InfiniteGas) == InfiniteGas)
			{
				list.Add('g');
			}
			if ((val & InfiniteBlades) == InfiniteBlades)
			{
				list.Add('b');
			}
			if ((val & InfiniteAhssAmmo) == InfiniteAhssAmmo)
			{
				list.Add('a');
			}
			return list;
		}

		public static int ToInt()
		{
			return 0;
		}
	}
}
