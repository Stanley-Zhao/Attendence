using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using CARS.SourceCode;

namespace CARS.Control
{
	public partial class NumberColorBars : UserControl
	{
		public NumberColorBars()
		{
			InitializeComponent();

			NumberColorBar sickBar = null;// new NumberColorBar(LeaveType.Sick, 80); //80 hours sick leave
			NumberColorBar annualBar = null;// new NumberColorBar(LeaveType.Annual, 120); //120 hours annual leave

			sickBar.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			sickBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
			sickBar.AskedLeave = 64;
			annualBar.VerticalAlignment = System.Windows.VerticalAlignment.Center;
			annualBar.HorizontalAlignment = System.Windows.HorizontalAlignment.Left;

			Grid.SetRow(sickBar, 0);
			Grid.SetRow(annualBar, 1);

			LayoutRoot.Children.Add(sickBar);
			LayoutRoot.Children.Add(annualBar);
		}
	}
}
