using JiraWhatsAppAssessment.Api.Models;

namespace JiraWhatsAppAssessment.Api.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendToRahulAsync(TicketAssessment ticket)
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

        // POC mock boundary. Replace this method with the approved WhatsApp provider integration.
        _logger.LogInformation("Mock WhatsApp notification to Rahul:\n{Message}", message);
        return Task.CompletedTask;
    }
}
