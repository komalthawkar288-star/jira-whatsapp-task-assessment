using JiraWhatsAppAssessment.Api.Data;
using JiraWhatsAppAssessment.Api.Models;
using JiraWhatsAppAssessment.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace JiraWhatsAppAssessment.Api.Controllers;

[ApiController]
[Route("api/jira")]
public class JiraController : ControllerBase
{
    [HttpPost("webhook")]
    public async Task<IActionResult> Webhook(
        [FromBody] JiraWebhook payload,
        [FromServices] InMemoryStore store,
        [FromServices] AssessmentService assessmentService,
        [FromServices] NotificationService notificationService)
    {
        var issue = payload.Issue;
        var fields = issue?.Fields;
        var assignee = fields?.Assignee?.DisplayName ?? "";

        if (!assignee.Contains("Komal", StringComparison.OrdinalIgnoreCase))
            return Ok(new { ignored = true, reason = "Ticket is not assigned to Komal." });

        var status = fields?.Status?.Name ?? "Unknown";
        var latestUpdate = payload.Comment?.Body ?? "Jira ticket updated.";
        DateTime? dueDate = DateTime.TryParse(fields?.Duedate, out var parsed) ? parsed : null;
        var assessment = assessmentService.Assess(status, dueDate, latestUpdate);

        var ticket = new TicketAssessment(
            issue?.Key ?? "UNKNOWN",
            fields?.Summary ?? "",
            assignee,
            status,
            dueDate,
            latestUpdate,
            assessment.Assessment,
            assessment.Reason,
            DateTime.UtcNow);

        lock (store.Tickets) store.Tickets.Insert(0, ticket);
        await notificationService.SendToRahulAsync(ticket);

        return Ok(ticket);
    }
}
