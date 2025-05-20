using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;

public class EmailSender : IEmailSender
{
    private readonly IConfiguration _config;

    public EmailSender(IConfiguration config)
    {
        _config = config;
    }

    public async Task SendEmailAsync(string email, string subject, string htmlMessage)
    {
        var emailMessage = new MimeMessage();
        emailMessage.From.Add(new MailboxAddress("Computer Zone", _config["EmailSettings:From"]));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = htmlMessage };

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["EmailSettings:SmtpServer"], int.Parse(_config["EmailSettings:Port"]!), SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["EmailSettings:Username"], _config["EmailSettings:Password"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}
