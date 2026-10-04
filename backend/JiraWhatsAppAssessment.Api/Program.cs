using JiraWhatsAppAssessment.Api.Data;
using JiraWhatsAppAssessment.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddSingleton<AssessmentService>();
builder.Services.AddSingleton<NotificationService>();
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
    policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod()));

var app = builder.Build();
app.UseCors();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
