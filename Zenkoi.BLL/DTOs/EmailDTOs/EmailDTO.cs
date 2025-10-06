using MimeKit;

namespace Zenkoi.BLL.DTOs.EmailDTOs
{
	public class EmailDTO
	{
		public List<MailboxAddress> To { get; set; }
		public string Subject { get; set; }
		public string Body { get; set; }
		public EmailDTO(IEnumerable<string> to, string subject, string body)
		{
			To = new List<MailboxAddress>();
			To.AddRange(to.Select(x => new MailboxAddress("email", x)));
			Subject = subject;
			Body = body;
		}

	}

	public class SendEmailRequestDTO
	{
		public List<string> ToEmails { get; set; } = new List<string>();
		public string Subject { get; set; }
		public string Body { get; set; }
	}
}
