using BazaR.Backend.Infrastructure;
using MediatR;
//using Microsoft.AspNetCore.Authentication.JwtBearer;

var builder = WebApplication.CreateBuilder(args);

// ==========================
// Services
// ==========================

// MVC Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Application (MediatR)
// –егистрируем хендлеры, сканиру€ сборку Application.
// —амый надЄжный вариант Ч указать любой тип из Application assembly.
builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(BazaR.Backend.Application.AssemblyReference).Assembly)
);

// Infrastructure (DbContext + repositories + UnitOfWork)
builder.Services.AddInfrastructure(builder.Configuration);

/*// Auth (пока можешь оставить заглушкой)
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer();*/

builder.Services.AddAuthorization();

var app = builder.Build();

// ==========================
// Pipeline
// ==========================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
