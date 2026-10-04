using JiraWhatsAppAssessment.Api.Models;

namespace JiraWhatsAppAssessment.Api.Data;

public class InMemoryStore
{
    public List<TicketAssessment> Tickets { get; } = new();
}
