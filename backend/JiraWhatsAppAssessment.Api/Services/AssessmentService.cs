using JiraWhatsAppAssessment.Api.Models;

namespace JiraWhatsAppAssessment.Api.Services;

public class AssessmentService
{
    public AssessmentResult Assess(string status, DateTime? dueDate, string latestUpdate)
    {
        var text = latestUpdate.ToLowerInvariant();

        if (text.Contains("block") || text.Contains("dependency") || text.Contains("waiting"))
            return new("NOT ON TIME", "The latest Jira update indicates a blocker or unresolved dependency.");

        if (dueDate is null)
            return new("ON TIME", "No due date is available, so the current update is treated as on track.");

        var days = (dueDate.Value.Date - DateTime.UtcNow.Date).TotalDays;

        if (days < 0)
            return new("NOT ON TIME", "The ticket due date has already passed.");

        if (days >= 3 && (text.Contains("complete") || text.Contains("testing")))
            return new("BEFORE TIME", "Progress is ahead and sufficient time remains before the due date.");

        if (days <= 1 && !text.Contains("complete"))
            return new("NOT ON TIME", "The due date is very close and the latest update does not indicate completion.");

        return new("ON TIME", "Current progress appears consistent with the planned due date.");
    }
}
