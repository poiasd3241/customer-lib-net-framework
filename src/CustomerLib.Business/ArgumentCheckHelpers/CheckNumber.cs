using System;

namespace CustomerLib.Business.ArgumentCheckHelpers
{
	public class CheckNumber
	{
		public static void NotLessThan(int minValue, int value, string paramName)
		{
			if (value < minValue)
			{
				throw new ArgumentException($"Cannot be less than {minValue}.", paramName);
			}
		}
	}
}
