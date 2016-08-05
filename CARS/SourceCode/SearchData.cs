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
	public class SearchData
	{
		public string Supervisor { get; set; }
		public string Applicant { get; set; }
		public LeaveStatus Status { get; set; }
		public DateTime Start { get; set; }
		public DateTime End { get; set; }
	}
}
