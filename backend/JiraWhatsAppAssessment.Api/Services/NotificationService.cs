using JiraWhatsAppAssessment.Api.Models;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace JiraWhatsAppAssessment.Api.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public async Task SendToRahulAsync(TicketAssessment ticket)
    {
        var dueDate = ticket.DueDate?.ToString("yyyy-MM-dd") ?? "Not specified";

        var message =
            "JIRA TASK UPDATE\n" +
            $"Ticket: {ticket.TicketKey}\n" +
            $"Assignee: {ticket.Assignee}\n" +
            $"Status: {ticket.Status}\n" +
            $"Due Date: {dueDate}\n" +
            $"Latest Update: {ticket.LatestUpdate}\n" +
            $"Assessment: {ticket.Assessment}\n" +
            $"Reason: {ticket.Reason}";

        var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID");
        var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN");
        var from = Environment.GetEnvironmentVariable("TWILIO_WHATSAPP_FROM");
        var to = Environment.GetEnvironmentVariable("TWILIO_WHATSAPP_TO");

        if (string.IsNullOrWhiteSpace(accountSid) ||
            string.IsNullOrWhiteSpace(authToken) ||
            string.IsNullOrWhiteSpace(from) ||
            string.IsNullOrWhiteSpace(to))
        {
            throw new InvalidOperationException("Twilio environment variables are not configured.");
        }

        TwilioClient.Init(accountSid, authToken);

        var result = await MessageResource.CreateAsync(
            body: message,
            from: new PhoneNumber(from),
            to: new PhoneNumber(to));

        _logger.LogInformation(
            "WhatsApp notification sent successfully. SID: {MessageSid}",
            result.Sid);
    }
}
