using JiraWhatsAppAssessment.Api.Models;

namespace JiraWhatsAppAssessment.Api.Services;

public class AssessmentService
{
    public AssessmentResult Assess(string status, DateTime? dueDate, string latestUpdate)
    {
        var text = latestUpdate.ToLowerInvariant();
        var isDone = status.Equals("Done", StringComparison.OrdinalIgnoreCase);

        if (text.Contains("block") || text.Contains("dependency") || text.Contains("waiting"))
            return new("NOT ON TIME", "The latest Jira update indicates a blocker or unresolved dependency.");

        if (dueDate is null)
            return new("ON TIME", "No due date is available, so the current update is treated as on track.");

        var days = (dueDate.Value.Date - DateTime.UtcNow.Date).TotalDays;

        // Completed before the planned due date.
        if (isDone && days > 0)
            return new("BEFORE TIME", "The task was completed before the planned due date.");

        // Completed exactly on the planned due date.
        if (isDone && days == 0)
            return new("ON TIME", "The task was completed on the planned due date.");

        // Due date has passed and the task is still not completed.
        if (days < 0 && !isDone)
            return new("NOT ON TIME", "The ticket due date has passed and the task is not completed.");

        // A task that is not completed and is due today is at risk of missing delivery.
        if (days == 0 && !isDone)
            return new("NOT ON TIME", "The task is due today and is not completed yet.");

        // Progress indicates completion/testing well before the due date.
        if (days >= 3 && (text.Contains("complete") || text.Contains("testing")))
            return new("BEFORE TIME", "Progress is ahead and sufficient time remains before the due date.");

        return new("ON TIME", "Current progress appears consistent with the planned due date.");
    }
}
