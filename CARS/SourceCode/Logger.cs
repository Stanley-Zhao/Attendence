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
using System.IO.IsolatedStorage;
using System.Text;

namespace CARS.SourceCode
{
	public class Logger
	{
		private static Logger logger;
		private string fileName;

		private Logger()
		{
			fileName = "CARS_SL_" + DateTime.Now.ToString("yyyy_MM_dd") + ".log";
		}

		public static Logger Instance()
		{
			if (logger == null)
				logger = new Logger();
			return logger;
		}

		public void Start()
		{
			Log(Environment.NewLine + ">>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>  Start");
		}

		private void Log(string log)
		{
			//Get　the　storage　file　for　the　application

			//Step1,　you　need　to　access　an　IsolatedStorageFile.
			//This　is　done　by　calling IsolatedStorageFile.GetUserStoreForApplication()　or　IsolatedStorageFile.GetUserStoreForSite().
			//There　are　some　differences　between　the　functions,　but　either　will　get　you　a　private　file　system　in　which　you　can　read　and　write　files.
			IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication(); //　Open&Create　the　file　for　writing

			//Step2,　you　need　create　or　open　your　file　almost　as　you　would　normally.
			//Instead　of　using　FileStream,　you　would　use　IsolatedStorageFileStream　(which　is　derived　from　FileStream).
			//This　means　that　once　you　have　your　stream,　you　can　use　it　anywhere　you　would　have　normally　used　a　normal　FileStream.
			IsolatedStorageFileStream stream = new IsolatedStorageFileStream(fileName, System.IO.FileMode.Append, System.IO.FileAccess.Write, isf); //　Use　the　stream　normally　in　a　TextWriter

			//Step3,　you　read　or　write　as　you　normally　would.
			System.IO.TextWriter writer = new System.IO.StreamWriter(stream);
			writer.WriteLine(log);

			writer.Close();　//　Close　the　writer　so　data　is　flushed
			stream.Close();　//　Close　the　stream　too
		}

		public void Log(MessageType type, string log)
		{
			StringBuilder sb = new StringBuilder();
			switch (type)
			{
				case MessageType.Information:
					sb.AppendFormat("[{0} {1}]", type.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));					
					break;
				case MessageType.Warning:
					sb.AppendFormat("<{0} {1}>", type.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
					break;
				case MessageType.Error:
					sb.AppendFormat("****{0} {1}****", type.ToString(), DateTime.Now.ToString("yyyy-MM-dd hh:mm:ss"));
					break;
			}
			sb.AppendLine();
			sb.Append("\t");
			sb.AppendLine(log);
			Log(sb.ToString());
		}
	}
}
