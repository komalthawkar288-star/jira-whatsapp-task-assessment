using JiraWhatsAppAssessment.Api.Data;
using JiraWhatsAppAssessment.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddSingleton<InMemoryStore>();
builder.Services.AddSingleton<AssessmentService>();
builder.Services.AddSingleton<NotificationService>();

var allowedOrigins = builder.Configuration["ALLOWED_ORIGINS"];
builder.Services.AddCors(options => options.AddDefaultPolicy(policy =>
{
    if (string.IsNullOrWhiteSpace(allowedOrigins))
        policy.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    else
        policy.WithOrigins(allowedOrigins.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
              .AllowAnyHeader().AllowAnyMethod();
}));

var app = builder.Build();
app.UseCors();
app.MapControllers();
app.MapGet("/health", () => Results.Ok(new { status = "ok" }));
app.Run();
