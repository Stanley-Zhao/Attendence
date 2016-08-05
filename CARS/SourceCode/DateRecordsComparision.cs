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
using System.Collections;
using System.Collections.Generic;

namespace CARS.SourceCode
{
	public class DateRecordsComparision : IComparer<DateRecords> 
	{
		public int Compare(DateRecords x, DateRecords y)
		{
			return x.StartTime.CompareTo(y.StartTime);
		}
	}
}
