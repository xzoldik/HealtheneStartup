using BusinessLogic;
using DataAccess;
using Domain.Interfaces;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddScoped<IAuthRepo, AuthRepo>();
builder.Services.AddScoped<IQuestionnaireRepo, QuestionnaireRepo>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<QuestionnaireService>();
builder.Services.AddScoped<ITherapist, TherapistRepo>();
builder.Services.AddScoped<TherapistService>();





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
