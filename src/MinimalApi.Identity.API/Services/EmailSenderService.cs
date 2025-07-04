﻿using MailKit.Net.Smtp;
using Microsoft.Extensions.Options;
using MimeKit;
using MinimalApi.Identity.API.Options;
using MinimalApi.Identity.API.Services.Interfaces;
using MinimalApi.Identity.Core.Entities;
using MinimalApi.Identity.Core.Enums;

namespace MinimalApi.Identity.API.Services;

public class EmailSenderService(IEmailSavingService emailSaving, IOptions<SmtpOptions> smtpOptions) : IEmailSenderService
{
    public async Task SendEmailTypeAsync(string email, string callbackUrl, EmailSendingType typeSender)
        => await MailKitSendEmailAsync(email, "Confirm your email",
            $"Please confirm your account by <a href='{callbackUrl}'>clicking here</a>.", typeSender);

    public async Task SendEmailAsync(string email, string subject, string message, EmailSendingType typeSender)
        => await MailKitSendEmailAsync(email, subject, message, typeSender);

    private async Task MailKitSendEmailAsync(string emailTo, string emailSubject, string emailMessage, EmailSendingType emailSendingType)
    {
        var options = smtpOptions.Value;

        using SmtpClient client = new();
        MimeMessage message = new();

        try
        {
            await client.ConnectAsync(options.Host, options.Port, options.Security);

            if (!string.IsNullOrEmpty(options.Username))
            {
                await client.AuthenticateAsync(options.Username, options.Password);
            }

            message.From.Add(MailboxAddress.Parse(options.Sender));
            message.To.Add(MailboxAddress.Parse(emailTo));
            message.Subject = emailSubject;
            message.Body = new TextPart("html") { Text = emailMessage };

            await client.SendAsync(message);
            await client.DisconnectAsync(true);

            if (options.SaveEmailSent)
            {
                await SaveEmailAsync(emailTo, emailSubject, emailMessage, emailSendingType, true, null, null);
            }
        }
        catch (Exception ex)
        {
            await SaveEmailAsync(emailTo, emailSubject, emailMessage, emailSendingType, false, "Error sending email", ex.Message);
        }
    }

    private async Task SaveEmailAsync(string emailTo, string emailSubject, string emailMessage, EmailSendingType emailSendingType, bool sent, string? errorMessage, string? errorDetails)
    {
        var emailSending = new EmailSending
        {
            EmailSendingType = emailSendingType,
            EmailTo = emailTo,
            Subject = emailSubject,
            Body = emailMessage,
            Sent = sent,
            DateSent = DateTime.UtcNow,
            ErrorMessage = errorMessage,
            ErrorDetails = errorDetails
        };

        await emailSaving.SaveEmailAsync(emailSending);
    }
}