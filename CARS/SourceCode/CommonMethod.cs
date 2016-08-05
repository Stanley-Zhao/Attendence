using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CARS.SourceCode
{
	public static class CommonMethods
	{		
		public static int ComputeHours(DateTime startTime, DateTime endTime)
		{
			int hours = 0;
			if (startTime.Hour < 9)
				startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day, 9, 0, 0);
			else if (startTime.Hour > 17)
				startTime = new DateTime(startTime.Year, startTime.Month, startTime.Day + 1, 9, 0, 0);
			if (endTime.Hour > 17 && endTime.Hour < 24)
				endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day, 17, 0, 0);
			else if (endTime.Hour < 9)
				endTime = new DateTime(endTime.Year, endTime.Month, endTime.Day - 1, 17, 0, 0);

			// if in same day
			if (startTime.Year == endTime.Year && startTime.DayOfYear == endTime.DayOfYear)
			{
				hours = endTime.Hour - startTime.Hour;
			}
			else // not in the same day
			{
				//  **Don't care about the weekend in current version**
				int days = endTime.DayOfYear - startTime.DayOfYear - 1;

				hours = days * 8 + (17 - startTime.Hour) + (endTime.Hour - 9);
			}
			return hours;
		}
	}
}
