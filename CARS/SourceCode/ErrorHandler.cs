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
using CARS.Control;

namespace CARS.SourceCode
{
	public class ErrorHandler
	{
		public static bool Handle(Exception e)
		{
			if (e != null)
			{
				Message.Error(e.Message);
				Logger.Instance().Log(MessageType.Error, e.Message + Environment.NewLine + e.StackTrace);

				return true; // has error
			}
			else
			{
				return false; // no error
			}
		}
	}
}
