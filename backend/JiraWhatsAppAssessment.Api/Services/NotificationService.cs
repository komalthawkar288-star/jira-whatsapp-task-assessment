using System.Net.Http.Headers;
using System.Text;
using JiraWhatsAppAssessment.Api.Models;

namespace JiraWhatsAppAssessment.Api.Services;

// Calls the Twilio REST API directly (Body/From/To only), bypassing the Twilio SDK.
// Error 21654 means the request carried ContentVariables without ContentSid;
// a plain form post cannot include them.
public class NotificationService
{
    private static readonly HttpClient Http = new() { Timeout = TimeSpan.FromSeconds(15) };
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

        if (message.Length > 1500)
            message = message[..1500] + "...";

        var sid = Environment.GetEnvironmentVariable("TWILIO_ACCOUNT_SID")?.Trim();
        var token = Environment.GetEnvironmentVariable("TWILIO_AUTH_TOKEN")?.Trim();
        var from = Environment.GetEnvironmentVariable("TWILIO_WHATSAPP_FROM")?.Trim();
        var to = Environment.GetEnvironmentVariable("TWILIO_WHATSAPP_TO")?.Trim();

        if (string.IsNullOrWhiteSpace(sid) || string.IsNullOrWhiteSpace(token) ||
            string.IsNullOrWhiteSpace(from) || string.IsNullOrWhiteSpace(to))
        {
            _logger.LogError("Twilio environment variables are not configured.");
            return;
        }

        try
        {
            var url = $"https://api.twilio.com/2010-04-01/Accounts/{sid}/Messages.json";
            using var request = new HttpRequestMessage(HttpMethod.Post, url)
            {
                Content = new FormUrlEncodedContent(new Dictionary<string, string>
                {
                    ["From"] = WithPrefix(from),
                    ["To"] = WithPrefix(to),
                    ["Body"] = message
                })
            };
            request.Headers.Authorization = new AuthenticationHeaderValue(
                "Basic", Convert.ToBase64String(Encoding.ASCII.GetBytes($"{sid}:{token}")));

            using var response = await Http.SendAsync(request);
            var body = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
                _logger.LogInformation("WhatsApp notification sent. Response: {Body}", body);
            else
                _logger.LogError("Twilio rejected the message. HTTP {Status}: {Body}",
                    (int)response.StatusCode, body);
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
