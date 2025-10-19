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
			var maxRetries = 3;
			var retryDelay = 2000; // 2 seconds

			for (int attempt = 1; attempt <= maxRetries; attempt++)
			{
				try
				{
					Console.WriteLine($"Email attempt {attempt}/{maxRetries}");
					Send(emailMessage);
					
					return new BaseResponse
					{
						IsSuccess = true,
						Message = "Email sent successfully"
					};
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Email attempt {attempt} failed: {ex.Message}");
					
					if (attempt == maxRetries)
					{
						return new BaseResponse
						{
							IsSuccess = false,
							Message = $"Không thể gửi email sau {maxRetries} lần thử: {ex.Message}"
						};
					}
					
					// Wait before retry
					Thread.Sleep(retryDelay * attempt);
				}
			}

			return new BaseResponse
			{
				IsSuccess = false,
				Message = "Không thể gửi email"
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
				// Configure timeouts
				client.Timeout = 30000; // 30 seconds
				
				Console.WriteLine($"Connecting to SMTP server: {_emailConfig.SmtpServer}:{_emailConfig.Port}");
				client.Connect(_emailConfig.SmtpServer, _emailConfig.Port, true);
				
				Console.WriteLine("SMTP connection established");
				client.AuthenticationMechanisms.Remove("XOAUTH2");
				
				Console.WriteLine("Authenticating with SMTP server...");
				client.Authenticate(_emailConfig.UserName, _emailConfig.Password);
				
				Console.WriteLine("Sending email...");
				client.Send(mailMessage);
				Console.WriteLine("Email sent successfully");
			}
			catch (Exception ex)
			{
				Console.WriteLine($"SMTP Error: {ex.Message}");
				Console.WriteLine($"SMTP Stack Trace: {ex.StackTrace}");
				throw new Exception($"Không thể gửi email: {ex.Message}", ex);
			}
			finally
			{
				try
				{
					client.Disconnect(true);
				}
				catch (Exception ex)
				{
					Console.WriteLine($"Error disconnecting SMTP: {ex.Message}");
				}
				client.Dispose();
			}
		}
	}
}
