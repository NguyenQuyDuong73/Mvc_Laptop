Support for ASP.NET Core Identity was added to your project.

For setup and configuration information, see https://go.microsoft.com/fwlink/?linkid=2116645.
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

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
        emailMessage.From.Add(new MailboxAddress("Computer Zone", _config["nguyenquyduong14@gmail.com"]));
        emailMessage.To.Add(MailboxAddress.Parse(email));
        emailMessage.Subject = subject;
        emailMessage.Body = new TextPart("html") { Text = htmlMessage };

        using var client = new SmtpClient();
        await client.ConnectAsync(_config["smtp.gmail.com"],
                                  int.Parse(_config["587"]),
                                  SecureSocketOptions.StartTls);
        await client.AuthenticateAsync(_config["nguyenquyduong14@gmail.com"],
                                       _config["qrf ecnw ekcr juku"]);
        await client.SendAsync(emailMessage);
        await client.DisconnectAsync(true);
    }
}
