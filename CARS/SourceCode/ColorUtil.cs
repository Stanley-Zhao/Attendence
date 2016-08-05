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

namespace CARS.SourceCode
{
	public class ColorUtil
	{
		public static Color GetColorFromString(string colorValue)
		{
			try
			{
				colorValue = colorValue.Replace("#", "0x");
				int intValue = Convert.ToInt32(colorValue, 16);
				byte r = (byte)((intValue & 0xFF0000) >> 16);
				byte g = (byte)((intValue & 0x00FF00) >> 8);
				byte b = (byte)(intValue & 0x0000FF);
				return Color.FromArgb(0xFF, r, g, b);
			}
			catch
			{
				return Color.FromArgb(0, 0, 0, 0);
			}
		}

		public static Color GetColorFromString(string colorValue, string alphaValue)
		{
			try
			{
				colorValue = colorValue.Replace("#", "0x");
				alphaValue = alphaValue.Replace("#", "0x");
				int intValue = Convert.ToInt32(colorValue, 16);
				byte byteValue = (byte)Convert.ToInt32(alphaValue, 16);
				byte r = (byte)((intValue & 0xFF0000) >> 16);
				byte g = (byte)((intValue & 0x00FF00) >> 8);
				byte b = (byte)(intValue & 0x0000FF);
				return Color.FromArgb(byteValue, r, g, b);
			}
			catch
			{
				return Color.FromArgb(0, 0, 0, 0);
			}
		}

		public static Brush GetLinearGradientBrush(Color color1, int offset1, Color color2, int offset2, int angle)
		{
			GradientStop gs1 = new GradientStop();
			gs1.Color = color1;
			gs1.Offset = offset1;
			GradientStop gs2 = new GradientStop();
			gs2.Color = color2;
			gs2.Offset = offset2;
			GradientStopCollection gsc = new GradientStopCollection();
			gsc.Add(gs1);
			gsc.Add(gs2);
			return new LinearGradientBrush(gsc, angle);
		}

		public static Color WhiteSmoke
		{
			get { return GetColorFromString("#F5F5F5"); }
		}

		public static Color NormalGary
		{
			get { return GetColorFromString("#333333"); }
		}

		public static Color BrightRed
		{
			get { return GetColorFromString("#FF0033"); }
		}

		public static Color BrightBlue
		{
			get { return GetColorFromString("#0066CC"); }
		}

		public static Color BrightYellow
		{
			get { return GetColorFromString("#FFCC00"); }
		}

		public static Color BrightGreen
		{
			get { return GetColorFromString("#339933"); }
		}

		public static Color LightBrightRed
		{
			get { return GetColorFromString("#FF0033", "#99"); }
		}

		public static Color LightBrightBlue
		{
			get { return GetColorFromString("#0066CC", "#99"); }
		}

		public static Color LightBrightYellow
		{
			get { return GetColorFromString("#FFCC00", "#99"); }
		}

		public static Color LightBrightGreen
		{
			get { return GetColorFromString("#339933", "#99"); }
		}

		public static Color SlateGary
		{
			get { return GetColorFromString("#708090"); }
		}
	}
}
