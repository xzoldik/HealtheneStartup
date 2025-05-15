using BusinessLogic.Services;
using DataAccess.Repositories;
using Domain.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthRepo, AuthRepository>();
builder.Services.AddScoped<IQuestionnaireRepo, QuestionnaireRepository>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<QuestionnaireService>();
builder.Services.AddScoped<ITherapist, TherapistRepository>();
builder.Services.AddScoped<TherapistService>();
builder.Services.AddScoped<ISessionRepository, SessionRepository>();
builder.Services.AddScoped<SessionService>();






var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
