using JiraWhatsAppAssessment.Api.Data;
using Microsoft.AspNetCore.Mvc;

namespace JiraWhatsAppAssessment.Api.Controllers;

[ApiController]
[Route("api/tickets")]
public class TicketsController : ControllerBase
{
    [HttpGet]
    public IActionResult Get([FromServices] InMemoryStore store)
    {
        lock (store.Tickets) return Ok(store.Tickets.ToArray());
    }
}
