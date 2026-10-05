using JiraWhatsAppAssessment.Api.Models;
using Twilio;
using Twilio.Exceptions;
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

        // WhatsApp body limit is 1600 characters
        if (message.Length > 1500)
            message = message[..1500] + "...";

        var accountSid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID")?.Trim();
        var authToken = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")?.Trim();
        var from = Environment.GetEnvironmentVariable("TWILIO_WHATSAPP_FROM")?.Trim();
        var to = Environment.GetEnvironmentVariable("TWILIO_WHATSAPP_TO")?.Trim();

        if (string.IsNullOrWhiteSpace(accountSid) ||
            string.IsNullOrWhiteSpace(authToken) ||
            string.IsNullOrWhiteSpace(from) ||
            string.IsNullOrWhiteSpace(to))
        {
            _logger.LogError("Twilio environment variables are not configured.");
            return;
        }

        from = WithPrefix(from);
        to = WithPrefix(to);

        try
        {
            TwilioClient.Init(accountSid, authToken);

            var result = await MessageResource.CreateAsync(
                body: message,
                from: new PhoneNumber(from),
                to: new PhoneNumber(to));

            _logger.LogInformation(
                "WhatsApp notification sent. SID: {MessageSid}, Status: {Status}",
                result.Sid, result.Status);
        }
        catch (ApiException ex)
        {
            // Do not rethrow: avoids a 500 to Jira, which would trigger repeated retries.
            _logger.LogError(
                "Twilio rejected the message. Code: {Code}, Message: {Message}, Info: {Info}",
                ex.Code, ex.Message, ex.MoreInfo);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unexpected error while sending WhatsApp notification.");
        }
    }

    private static string WithPrefix(string number) =>
        number.StartsWith("whatsapp:", StringComparison.OrdinalIgnoreCase)
            ? number
            : $"whatsapp:{number}";
}
