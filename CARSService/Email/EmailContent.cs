using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CARSService.Email
{
	public class EmailContent
	{	
		public string Title { get; set; }
		public string Body { get; set; }
		public string TableRow { get; set; }

		public EmailContent() : this(string.Empty, string.Empty) { }

		public EmailContent(string title, string body)
		{
			Title = title;
			Body = body;
		}
	}
}