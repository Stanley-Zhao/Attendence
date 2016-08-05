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
	public class LeaveTypeValue
	{
		private LeaveType type;
		public LeaveType TypeValue
		{
			get { return type; }
			set { type = value; }
		}

		public LeaveTypeValue() { }
		public LeaveTypeValue(LeaveType pType)
		{
			type = pType;
		}

		public override string ToString()
		{
			return type.Name;
		}
	}
}
