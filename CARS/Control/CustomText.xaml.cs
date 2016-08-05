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

namespace CARS.Control
{
	public partial class CustomText : UserControl
	{
		public string TextValue
		{
			get { return customText.Text; }
			set
			{
				customText.Text = value;
				if (value == "Applying")
				{
					customText.Foreground = new SolidColorBrush(Colors.Blue);
					customText.FontWeight = FontWeights.Bold;
				}
				else
				{
					customText.Foreground = new SolidColorBrush(Colors.Black);
					customText.FontWeight = FontWeights.Normal;
				}
			}
		}

		public CustomText()
		{
			InitializeComponent();
		}
	}
}
