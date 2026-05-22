using System;
using System.Net;
using System.Net.Mail;
using FakeWebShop.Contracts.Request.ContactRequest;
using FakeWebShop.Domain.Services.Interface_s;
using Microsoft.Extensions.Configuration;

namespace FakeWebShop.Domain.Services.Interface_s;

public class EmailService(IConfiguration configuration) : IEmailService
{
     public async Task SendContactMailAsync(ContactRequest request)
    {
        var smtpHost = configuration["Email:SmtpHost"];
        var smtpPort = int.Parse(configuration["Email:SmtpPort"]!);
        var smtpUser = configuration["Email:SmtpUser"];
        var smtpPassword = configuration["Email:SmtpPassword"];
        var toEmail = configuration["Email:ToEmail"];

        using var client = new SmtpClient(smtpHost, smtpPort)
        {
            EnableSsl = true,
            Credentials = new NetworkCredential(smtpUser, smtpPassword)
        };

        var mail = new MailMessage
        {
            From = new MailAddress(smtpUser!),
            Subject = $"Nieuw contactbericht van {request.FirstName} {request.LastName}",
            Body = $"""
            Naam: {request.FirstName} {request.LastName}
            Email: {request.Email}
            Telefoon: {request.Phone}

            Bericht:
            {request.Message}
            """,
            IsBodyHtml = false
        };

        mail.To.Add(toEmail!);
        mail.ReplyToList.Add(new System.Net.Mail.MailAddress(request.Email));

        await client.SendMailAsync(mail);
    }

   
}
