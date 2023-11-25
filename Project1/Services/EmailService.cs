using Project1.Services;
using System.Net;
using System.Net.Mail;

public class EmailService : IEmailService
{
    public Task SendEmailAsync(string email, string subject, string message)
    {
        var client = new SmtpClient("smtp.gmail.com", 587)
        {
            EnableSsl = true,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential("deshgasware@gmail.com", "ujil fazu lgtl abjo")
        };

        return client.SendMailAsync(
            new MailMessage(from: "deshgasware@gmail.com",
                            to: email,
                            subject,
                            message
                            ));
    }
}