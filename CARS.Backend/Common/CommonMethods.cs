using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CARS.Backend.Entity;
using System.Configuration;

namespace CARS.Backend.Common
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
				//int workingDays = 0;
				//int days = endTime.DayOfYear - startTime.DayOfYear;
				//DateTime day = startTime;
				//for (int i = 0; i < days - 1; i++)
				//{
				//    day = startTime.AddDays(i);
				//    if (day.DayOfWeek != DayOfWeek.Saturday && day.DayOfWeek != DayOfWeek.Sunday)
				//    {
				//        workingDays++;
				//    }
				//}

				//hours = workingDays * 8 + (17 - startTime.Hour) + (endTime.Hour - 9);

				//  **Don't care about the weekend in current version**
				int days = endTime.DayOfYear - startTime.DayOfYear - 1;

				hours = days * 8 + (17 - startTime.Hour) + (endTime.Hour - 9);
			}
			return hours;
		}

		public static int ComputeHours(List<TimeDurationInfo> timeDurationList)
		{
			if (timeDurationList == null)
				return 0;

			int hours = 0;
			foreach (TimeDurationInfo timeDuration in timeDurationList)
			{
				hours += CommonMethods.ComputeHours(timeDuration.StartTime, timeDuration.EndTime);
			}
			return hours;
		}

		public static int GetFrozenDay()
		{
			return Int32.Parse((ConfigurationManager.AppSettings[GlobalParams.FrozenDayNodeName]));
		}

		public static DateTime GetCurrentFrozenDate()
		{
			DateTime result = DateTime.Now;
			if (DateTime.Now.Day < GetFrozenDay() && DateTime.Now.Month > 1)
				result = new DateTime(DateTime.Now.Year, DateTime.Now.Month - 1, GetFrozenDay(), 23, 59, 59);
			else if (DateTime.Now.Day < GetFrozenDay() && DateTime.Now.Month == 1)
				result = new DateTime(DateTime.Now.Year, 1, 1, 0, 0, 0); // if current month is Jan. Frozen date should be this last year's 12/31
			else
				result = new DateTime(DateTime.Now.Year, DateTime.Now.Month, GetFrozenDay(), 0, 0, 0);

			return result;
		}
	}
}
