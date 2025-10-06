using MailKit.Net.Smtp;
using MimeKit;
using Zenkoi.BLL.DTOs.EmailDTOs;
using Zenkoi.BLL.DTOs.Response;
using Zenkoi.BLL.Helpers.Config;
using Zenkoi.BLL.Services.Interfaces;

namespace Zenkoi.BLL.Services.Implements
{
	public class EmailService : IEmailService
	{
		private readonly EmailConfiguration _emailConfig;

		public EmailService(EmailConfiguration emailConfig)
		{
			_emailConfig = emailConfig;
		}

		public BaseResponse SendEmail(EmailDTO emailDTO)
		{
			var emailMessage = CreateEmailMessage(emailDTO);
			Send(emailMessage);

			return new BaseResponse
			{
				IsSuccess = true,
				Message = "Email sent successfully"
			};
		}

		private MimeMessage CreateEmailMessage(EmailDTO emailDTO)
		{
			var emailMessage = new MimeMessage();
			emailMessage.From.Add(new MailboxAddress("Personal SMTP Gmail", _emailConfig.From));
			emailMessage.To.AddRange(emailDTO.To);
			emailMessage.Subject = emailDTO.Subject;
			emailMessage.Body = new TextPart(MimeKit.Text.TextFormat.Html) { Text = emailDTO.Body };

			return emailMessage;
		}

		private void Send(MimeMessage mailMessage)
		{
			using var client = new SmtpClient();
			try
			{
				client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
				client.AuthenticationMechanisms.Remove("XOAUTH2");
				client.Authenticate(_emailConfig.UserName, _emailConfig.Password);

				client.Send(mailMessage);
			}
			catch (Exception ex)
			{
				throw new Exception("An error occurred while sending the email: " + ex.Message, ex);
			}
			finally
			{
				client.Disconnect(true);
				client.Dispose();
			}
		}
	}
}
