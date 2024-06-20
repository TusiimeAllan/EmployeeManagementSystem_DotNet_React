using EmployeeManagementAPI.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using DotNetEnv;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using EmployeeManagementAPI;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
    policy =>
    {
        policy.WithOrigins("http://localhost:3000")
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowCredentials(); // Allowing credentials like cookies or authentication tokens
    });
});

// Load environment variables from .env file
Env.Load();

// Inject the logger service
var logger = builder.Services.BuildServiceProvider().GetRequiredService<ILogger<Program>>();

// JWT configuration
var jwtKey = JwtKeyGenerator.GenerateJwtKey();
var jwtIssuer = Environment.GetEnvironmentVariable("Jwt_Issuer");
var jwtAudience = Environment.GetEnvironmentVariable("Jwt_Audience");
var jwtExpireDays = int.Parse(Environment.GetEnvironmentVariable("Jwt_ExpireDays"));

var jwtSettings = new JwtSettings
{
    Key = jwtKey,
    Issuer = jwtIssuer,
    Audience = jwtAudience,
    ExpireDays = jwtExpireDays
};

// Log the JwtSettings details
logger.LogInformation($"JWT Settings: Key={jwtSettings.Key}, Issuer={jwtSettings.Issuer}, Audience={jwtSettings.Audience}, ExpireDays={jwtSettings.ExpireDays}");

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configure Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Configure JWT Authentication
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer("Bearer", options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey))
        };
        options.Authority = "http://localhost:3000";
        options.RequireHttpsMetadata = false;
        options.Audience = jwtSettings.Audience;
    });

builder.Services.AddAuthorization();

builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

var app = builder.Build();

// Error Handling Middleware
app.UseExceptionHandler("/error");
app.UseStatusCodePages();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
