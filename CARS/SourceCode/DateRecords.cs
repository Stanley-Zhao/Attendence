using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CARS.CARSService;

namespace CARS.SourceCode
{
	public class DateRecords
	{
		private TimeDurationInfo mTDI = new TimeDurationInfo();
		public TimeDurationInfo @TimeDurationInfo
		{
			get { return mTDI; }
		}

		public int Hours
		{
			get;
			set;
		}

		public string Record
		{
			get;
			set;
		}

		public DateRecords()
			: this(null)
		{
		}

		public DateRecords(TimeDurationInfo tdi)
		{
			if (tdi != null)
			{
				Hours = CommonMethods.ComputeHours(tdi.StartTime, tdi.EndTime);
				Record = tdi.StartTime.ToString("yyyy/MM/dd HH:00") + " - " + tdi.EndTime.ToString("yyyy/MM/dd HH:00");
				mTDI = tdi;
			}
		}

		public DateTime StartTime
		{
			get { return mTDI.StartTime; }
			set { mTDI.StartTime = value; }
		}

		public DateTime EndTime
		{
			get { return mTDI.EndTime; }
			set { mTDI.EndTime = value; }
		}

		public TimeDurationInfo ToTimeDurationInfo()
		{
			if (Hours > 0 && Record != string.Empty)
			{
				mTDI = new TimeDurationInfo();
				string[] times = Record.Split('-');
				DateTime startTime = Convert.ToDateTime(times[0].Trim());
				DateTime endTime = Convert.ToDateTime(times[1].Trim());
				mTDI.StartTime = startTime;
				mTDI.EndTime = endTime;
			}
			return mTDI;
		}

		public override string ToString()
		{
			return Record;
		}
	}
}
