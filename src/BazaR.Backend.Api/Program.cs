using BazaR.Backend.Infrastructure;
using BazaR.Backend.Infrastructure.Auth;
using BazaR.Backend.Infrastructure.Persistence;
using BazaR.Backend.Infrastructure.Persistence.Files;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);


// Services
builder.Services.AddControllers();
builder.Services.AddHttpContextAccessor();


// Swagger + Bearer
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter: Bearer <your_token>"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddMediatR(cfg =>
    cfg.RegisterServicesFromAssembly(typeof(BazaR.Backend.Application.AssemblyReference).Assembly)
);


builder.Services.AddInfrastructure(builder.Configuration);


builder.Services.Configure<LocalFileStorageOptions>(
    builder.Configuration.GetSection("Files:Local")
);




var allowedOrigins = builder.Configuration
    .GetSection("Cors:AllowedOrigins")
    .Get<string[]>() ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("Frontend", policy =>
    {
        
        if (allowedOrigins.Length == 0 && builder.Environment.IsDevelopment())
        {
            policy
                .AllowAnyOrigin()
                .AllowAnyHeader()
                .AllowAnyMethod();

            
            return;
        }

        policy
            .WithOrigins(allowedOrigins)
            .AllowAnyHeader()
            .AllowAnyMethod();

       
    });
});


// JWT Options binding

builder.Services.Configure<JwtOptions>(builder.Configuration.GetSection("Jwt"));


// JWT Authentication

var jwt = builder.Configuration.GetSection("Jwt").Get<JwtOptions>()
          ?? throw new InvalidOperationException("Jwt section missing");

if (string.IsNullOrWhiteSpace(jwt.SecretKey))
    throw new InvalidOperationException("Jwt:SecretKey missing");

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwt.SecretKey)),

            ValidateIssuer = !string.IsNullOrWhiteSpace(jwt.Issuer),
            ValidIssuer = jwt.Issuer,

            ValidateAudience = !string.IsNullOrWhiteSpace(jwt.Audience),
            ValidAudience = jwt.Audience,

            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2)
        };
    });


// Authorization

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("Admin", p => p.RequireRole("Admin"));
    options.AddPolicy("Seller", p => p.RequireRole("Seller", "Admin"));
    options.AddPolicy("Customer", p => p.RequireRole("Customer", "Seller", "Admin"));
});


var app = builder.Build();


// Static files for uploads

var fileOptions = app.Services.GetRequiredService<IOptions<LocalFileStorageOptions>>().Value;

if (string.IsNullOrWhiteSpace(fileOptions.BasePath))
    throw new InvalidOperationException("Files:Local:BasePath is required.");

if (string.IsNullOrWhiteSpace(fileOptions.RequestPath))
    throw new InvalidOperationException("Files:Local:RequestPath is required (example: /uploads).");

var requestPath = fileOptions.RequestPath.StartsWith("/")
    ? fileOptions.RequestPath
    : "/" + fileOptions.RequestPath;

//  делаем BasePath абсолютным 
var absoluteBasePath = Path.IsPathRooted(fileOptions.BasePath)
    ? fileOptions.BasePath
    : Path.GetFullPath(Path.Combine(app.Environment.ContentRootPath, fileOptions.BasePath));

Directory.CreateDirectory(absoluteBasePath);

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(absoluteBasePath),
    RequestPath = requestPath
});


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("Frontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();


/*using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}*/

app.Run();