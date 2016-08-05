using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using System.Timers;
using System.Configuration;

namespace CARSService.Email
{
	public class EmailConfig
	{
		private static Dictionary<string, EmailContent> dics = null;
		private static string apPath = string.Empty;
		private static string templdatesFolders = "emails";
		//private static Timer timer = null;
		private static string host;
		private static string emailAccount;
		private static string password;
		private static string timeFormatInEmail = string.Empty;
		private static string carsAddress = string.Empty;

		public static void SetRootPath(string pathValue)
		{
			apPath = pathValue;
		}

		public static void Start()
		{
			//            if (timer == null)
			//            {
			//#if DEBUG
			//                timer = new Timer(30 * 1000); // 30 secs
			//#else
			//                timer = new Timer(300*1000); // 5 mins
			//#endif


			//                timer.Elapsed += new ElapsedEventHandler(timer_Elapsed);
			// do it now on first time
			RefreshData();

			// start timer
			//timer.Start();
			//}
		}

		//private static void timer_Elapsed(object sender, ElapsedEventArgs e)
		//{
		//    RefreshData(); // every 5 mins, update email templates list.
		//}

		public static Dictionary<string, EmailContent> EmailTemplates
		{
			get
			{
				if (string.IsNullOrEmpty(apPath))
					throw new Exception("apPath is not set");

				templdatesFolders = Path.Combine(apPath, templdatesFolders);
				Start();
				return dics;
			}
		}

		public static void RefreshData()
		{
			if (dics == null) // load files when first time calling			
				dics = new Dictionary<string, EmailContent>();
			else
				dics.Clear();

			if (string.IsNullOrEmpty(apPath))
				throw new Exception("apPath is not set");

			templdatesFolders = Path.Combine(apPath, templdatesFolders);

			foreach (string s in Enum.GetNames(typeof(EmailType)))
			{
				string templateEmail = Path.Combine(templdatesFolders, s + ".txt");
				EmailContent email = CreateEmailTemplate(templateEmail);
				if (email != null)
				{
					if (!dics.ContainsKey(s))
						dics.Add(s, email);
					else
						dics[s] = email;
				}
			}

			// get Email Sender configuration
			//string senderConfigFile = Path.Combine(templdatesFolders, "EmailSenderConfig.txt");
			//StreamReader sr = new StreamReader(senderConfigFile);
			//emailAccount = sr.ReadLine().Replace("-> ", "");
			//password = sr.ReadLine().Replace("-> ", "");
			//host = sr.ReadLine().Replace("-> ", "");
			emailAccount = ConfigurationManager.AppSettings["DefaultEmailAccount"];
			password = ConfigurationManager.AppSettings["DefaultEmailPassword"];
			host = ConfigurationManager.AppSettings["SMPTHost"];
			timeFormatInEmail = ConfigurationManager.AppSettings["TimeFormatInEmail"];
			carsAddress = ConfigurationManager.AppSettings["CARSAddress"];
		}

		private static EmailContent CreateEmailTemplate(string file)
		{
			if (!File.Exists(file))
				return null;

			StreamReader sr = new StreamReader(file);
			EmailContent email = new EmailContent();
			email.Title = sr.ReadLine().Replace("-> ", "");
			email.Body = sr.ReadLine().Replace("-> ", "");
			email.TableRow = sr.ReadLine().Replace("-> ", "");
			sr.Close();
			return email;
		}

		public static string EmailAccount
		{
			get { Start(); return emailAccount; }
		}

		public static string Host
		{
			get { Start(); return host; }
		}

		public static string Password
		{
			get { Start(); return password; }
		}

		public static string TimeFormatInEmail
		{
			get { Start(); return timeFormatInEmail; }
		}

		public static string CARSAddress
		{
			get { Start(); return carsAddress; }
		}
	}
}