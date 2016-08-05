using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CARS.Backend.Entity;
using CARS.Backend.DAL;

namespace CARS.Backend.BLL
{
	public class ReportBLL
	{
		private static Dictionary<Common.MonthRank, ReportPeriod> rpList = new Dictionary<Common.MonthRank, ReportPeriod>();
		#region Service Methods

		public static List<ReportPeriod> GetReportPeriods()
		{
			List<ReportPeriod> result = CommonDAL<ReportPeriod>.GetObjects(null);

			for (int i = 0; i < result.Count; i++)
			{
				ReportPeriod item = result[i];

				if (item.StartTime == DateTime.MinValue)
					item.StartTime = null;

				if (item.EndTime == DateTime.MinValue)
					item.EndTime = null;

				if (rpList.Keys.Contains<Common.MonthRank>(item.Month))
					rpList[item.Month] = item;
				else
					rpList.Add(item.Month, item);
			}

			return result;
		}

		public static void UpdateReportPeriods(List<ReportPeriod> reportPeriods)
		{
			if (null != reportPeriods & reportPeriods.Count > 0)
			{
				foreach (ReportPeriod item in reportPeriods)
				{
					if (rpList.Keys.Contains<Common.MonthRank>(item.Month))
					{
						rpList[item.Month].StartTime = item.StartTime;
						rpList[item.Month].EndTime = item.EndTime;
					}
					else
					{
						rpList.Add(item.Month, item);
					}
				}

				foreach (KeyValuePair<Common.MonthRank, ReportPeriod> kVP in rpList)
				{
					kVP.Value.Save();
				}
			}
		}

		#endregion
	}
}
