using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using EquilibriumAPI.Data;
using EquilibriumAPI.Services;
using EquilibriumAPI.Services.Interfaces;
using EquilibriumAPI.Middleware;
using Microsoft.AspNetCore.RateLimiting;
using System.Threading.RateLimiting;

var builder = WebApplication.CreateBuilder(args);

// ── Banco de Dados (MySQL Local) ────────────────────────────────────
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

// ── JWT Authentication ────────────────────────────────────────────────────────
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var secretKey = Encoding.UTF8.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(secretKey),
            RoleClaimType = ClaimTypes.Role // Mapeia o claim Role padrão
        };
    });

builder.Services.AddAuthorization();

// ── CORS (permitir front-end Vue.js) ─────────────────────────────────────────
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>()
    ?? ["http://localhost:5173"];

builder.Services.AddCors(options =>
{
    options.AddPolicy("FrontEnd", policy =>
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// ── Services (Injeção de Dependência via Interfaces - OOP) ───────────────────
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEventosService, EventosService>();
builder.Services.AddScoped<ICuponsService, CuponsService>();
builder.Services.AddScoped<IPedidosService, PedidosService>();
builder.Services.AddScoped<IClientesService, ClientesService>();

// ── Rate Limiting (Proteção contra Força Bruta/DDoS) ─────────────────────────
builder.Services.AddRateLimiter(options =>
{
    options.AddFixedWindowLimiter("Global", opt =>
    {
        opt.Window = TimeSpan.FromMinutes(1);
        opt.PermitLimit = 60; // Máximo 60 requests por minuto por IP
        opt.QueueLimit = 2;
        opt.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    });
});

// ── Controllers + OpenAPI ────────────────────────────────────────────────────
builder.Services.AddControllers()
    .AddJsonOptions(opts =>
        opts.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase);
builder.Services.AddOpenApi();

// ── Build ─────────────────────────────────────────────────────────────────────
var app = builder.Build();

// ── Middlewares de Segurança ──────────────────────────────────────────────────
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();
app.UseMiddleware<SecurityHeadersMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
else
{
    // Força HTTPS em produção
    app.UseHttpsRedirection();
}

app.UseRateLimiter();
app.UseCors("FrontEnd");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Rota raiz amigável
app.MapGet("/", () => Results.Redirect("/swagger"));

app.Run();
