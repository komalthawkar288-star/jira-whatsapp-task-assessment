using JiraWhatsAppAssessment.Api.Models;

namespace JiraWhatsAppAssessment.Api.Services;

public class NotificationService
{
    private readonly ILogger<NotificationService> _logger;

    public NotificationService(ILogger<NotificationService> logger) => _logger = logger;

    public Task SendToRahulAsync(TicketAssessment ticket)
    {
        var message = $"""
        JIRA TASK UPDATE
        Ticket: {ticket.TicketKey}
        Assignee: {ticket.Assignee}
        Status: {ticket.Status}
        Due Date: {ticket.DueDate:yyyy-MM-dd}
        Latest Update: {ticket.LatestUpdate}
        Assessment: {ticket.Assessment}
        Reason: {ticket.Reason}
        """;

        // POC mock boundary. Replace this method with the approved WhatsApp provider integration.
        _logger.LogInformation("Mock WhatsApp notification to Rahul:
{Message}", message);
        return Task.CompletedTask;
    }
}
