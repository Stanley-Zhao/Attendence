using System;
using System.Net;
using System.Net.Mail;
using System.Net.Mime;
using System.Collections.Generic;
namespace CARSService.Email
{
	/**/
	/// <summary>  
	/// class of email sender  
	/// </summary>  
	public class SendMail
	{
		private MailMessage mailMessage;
		private SmtpClient smtpClient;
		private string password; //password of sender  
		private string host;
		/**/
		/// <summary>  
		/// construct of class of email sender  
		/// </summary>  
		/// <param name="to">to</param>  
		/// <param name="from">from</param>  
		/// <param name="body">mail body</param>  
		/// <param name="title">mail title</param>  
		/// <param name="pwd">password of sender</param>  
		public SendMail(string to, List<string> ccList, string from, string body, string title, string
pwd, string pHost)
		{
			mailMessage = new MailMessage();
			mailMessage.To.Add(to);
			if (ccList != null && ccList.Count > 0)
				foreach (string cc in ccList)
					mailMessage.CC.Add(cc);
			mailMessage.From = new System.Net.Mail.MailAddress(from);
#if !DEBUG
			mailMessage.Subject = title;			
#else
			mailMessage.Subject = "[DEBUG, only for SE testing, please delete this mail] " + title;
#endif
			mailMessage.Body = body;
			mailMessage.IsBodyHtml = true;
			mailMessage.BodyEncoding = System.Text.Encoding.UTF8;
			mailMessage.Priority = System.Net.Mail.MailPriority.Normal;
			this.password = pwd;
			this.host = pHost;
		}
		/**/
		/// <summary>  
		/// add attachment
		/// </summary>  
		public void Attachments(string pPath)
		{
			string[] paths = pPath.Split(',');
			Attachment data;
			ContentDisposition disposition;
			for (int i = 0; i < paths.Length; i++)
			{
				data = new Attachment(paths[i], MediaTypeNames.Application.Octet);//create a object of  
				disposition = data.ContentDisposition;
				disposition.CreationDate = System.IO.File.GetCreationTime(paths[i]);//get create date of attachment
				disposition.ModificationDate = System.IO.File.GetLastWriteTime(paths[i]);//get last modify date of attachment
				disposition.ReadDate = System.IO.File.GetLastAccessTime(paths[i]);//get last read date of attachment
				mailMessage.Attachments.Add(data);//add file to attachment  
			}
		}
		/**/
		/// <summary>  
		/// send mail in async
		/// </summary>  
		/// <param name="completedMethod"></param>  
		public void SendAsync(SendCompletedEventHandler completedMethod)
		{
			if (mailMessage != null)
			{
				smtpClient = new SmtpClient();
				smtpClient.Credentials = new System.Net.NetworkCredential
(mailMessage.From.Address, password);//set sender's credential
				smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				smtpClient.Host = "smtp." + mailMessage.From.Host;
				smtpClient.SendCompleted += new SendCompletedEventHandler
(completedMethod);//register the event of send email in async 
				smtpClient.SendAsync(mailMessage, mailMessage.Body);
			}
		}
		/**/
		/// <summary>  
		/// send mail
		/// </summary>  
		public void Send()
		{
			if (mailMessage != null)
			{
				smtpClient = new SmtpClient();
				smtpClient.Credentials = new System.Net.NetworkCredential
(mailMessage.From.Address, password);//set sender's credential  
				smtpClient.DeliveryMethod = System.Net.Mail.SmtpDeliveryMethod.Network;
				smtpClient.Host = host;
				smtpClient.Send(mailMessage);
			}
		}
	}
}