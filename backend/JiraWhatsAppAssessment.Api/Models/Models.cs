namespace JiraWhatsAppAssessment.Api.Models;

public record JiraWebhook(
    string? WebhookEvent,
    JiraIssue? Issue,
    JiraComment? Comment);

public record JiraIssue(string? Key, JiraFields? Fields);

public record JiraFields(
    string? Summary,
    JiraUser? Assignee,
    JiraStatus? Status,
    string? Duedate);

public record JiraUser(string? DisplayName);
public record JiraStatus(string? Name);
public record JiraComment(string? Body);

public record TicketAssessment(
    string TicketKey,
    string Summary,
    string Assignee,
    string Status,
    DateTime? DueDate,
    string LatestUpdate,
    string Assessment,
    string Reason,
    DateTime ProcessedAtUtc);

public record AssessmentResult(string Assessment, string Reason);
