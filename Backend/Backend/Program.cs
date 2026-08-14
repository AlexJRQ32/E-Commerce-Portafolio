using Backend.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Threading.RateLimiting;
using Microsoft.AspNetCore.HttpOverrides;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
        options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

builder.Services.AddOpenApi();

// CORS allowlist (not AllowAnyOrigin)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policyBuilder =>
    {
        policyBuilder
            .WithOrigins(
                "http://localhost:5173",      // Vite dev server
                "http://localhost:3000",      // Alternative dev port
                "https://e-commerce-portafolio.onrender.com"  // Production frontend
            )
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

// Rate limiting - partitioned by IP (not global)
builder.Services.AddRateLimiter(options =>
{
    // General app policy: 120 req/min per IP — applied via [EnableRateLimiting("general")] on public controllers
    options.AddPolicy("general", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 120,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    // Strict auth policy: 10 req/min per IP (anti brute-force)
    options.AddPolicy("auth-strict", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            partitionKey: context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            factory: _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 10,
                Window = TimeSpan.FromMinutes(1),
                QueueProcessingOrder = QueueProcessingOrder.OldestFirst,
                QueueLimit = 0
            }));

    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
});

// JWT with issuer/audience validation
var jwtKey = builder.Configuration["JWT_SECRET"] ?? throw new InvalidOperationException("JWT_SECRET no configurada");
var jwtIssuer = builder.Configuration["JWT_ISSUER"] ?? "api-comidas";
var jwtAudience = builder.Configuration["JWT_AUDIENCE"] ?? "app-comidas";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtKey))
        };
    });

// Connection string: prioritize env var DATABASE_URL over appsettings
var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
    ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
    throw new InvalidOperationException("No database connection string configured. Set DATABASE_URL env var or ConnectionStrings:DefaultConnection.");

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline

// N4 (FIX): Forwarded headers MUST be first — before HSTS, CORS, RateLimiter.
// ForwardLimit=2 trusts only the first 2 hops of X-Forwarded-For, preventing
// client-side spoofing while still working behind Cloudflare/Render edge proxies.
// KnownProxies is intentionally NOT set (would break dynamic edge IPs); ForwardLimit
// is the pragmatic mitigation against XFF spoofing.
app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
    ForwardLimit = 2
});

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // HSTS after ForwardedHeaders so the scheme is correctly resolved as https behind a proxy
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");
app.UseRateLimiter();

// Security headers middleware
app.Use(async (context, next) =>
{
    context.Response.Headers["X-Content-Type-Options"] = "nosniff";
    context.Response.Headers["X-Frame-Options"] = "DENY";
    context.Response.Headers["Referrer-Policy"] = "no-referrer";
    context.Response.Headers["Content-Security-Policy"] = "default-src 'self'";
    context.Response.Headers["Permissions-Policy"] = "geolocation=(), microphone=(), camera=()";
    context.Response.Headers["Cross-Origin-Opener-Policy"] = "same-origin";
    context.Response.Headers["Cross-Origin-Resource-Policy"] = "same-origin";
    await next();
});

app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();

// Make Program public for testing
public partial class Program { }
