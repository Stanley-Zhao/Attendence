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
	public partial class HolidayConfigControl : UserControl
	{
		public HolidayConfigControl(User user)
		{
			InitializeComponent();

			if (user.Type == UserType.Administrator || user.Type == UserType.ManagerAndAdmin)
				updateButton.Visibility = System.Windows.Visibility.Visible;
		}
	}
}
