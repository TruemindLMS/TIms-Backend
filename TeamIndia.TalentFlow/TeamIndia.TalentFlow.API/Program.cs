using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TeamIndia.TalentFlow.API.Extensions;
using TeamIndia.TalentFlow.Application.ApplicationSettings;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamIndia.TalentFlow.API", Version = "v1" });
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSettings>>().Value);
builder.Services.Configure<TeamIndia.TalentFlow.Application.ApplicationSettings.JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<TeamIndia.TalentFlow.Application.ApplicationSettings.JwtSettings>>().Value);
// bind seed admin settings
builder.Services.Configure<TeamIndia.TalentFlow.Application.ApplicationSettings.SeedAdminSettings>(builder.Configuration.GetSection("SeedAdmin"));

// Infrastructure/Application service registrations
builder.Services.AddMemoryCache();
builder.Services.AddScoped<Microsoft.AspNetCore.Identity.UserManager<TeamIndia.TalentFlow.Domain.Entities.ApplicationUser>>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.ITokenService, TeamIndia.TalentFlow.Application.Services.TokenService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IOtpService, TeamIndia.TalentFlow.Application.Services.OtpService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IAdminService, TeamIndia.TalentFlow.Application.Services.AdminService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IAuthService, TeamIndia.TalentFlow.Application.Services.AuthService>();
// Repositories
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IUserRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IRoleRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.RoleRepository>();

// Configure JWT authentication
var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSettings>() ?? new JwtSettings();
var key = Encoding.UTF8.GetBytes(jwtSettings.Key);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings.Issuer,
            ValidateAudience = true,
            ValidAudience = jwtSettings.Audience,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateLifetime = true,
            ClockSkew = System.TimeSpan.Zero
        };
    });

var app = builder.Build();

// run seeder
try
{
    TeamIndia.TalentFlow.API.Helpers.SeedDataHelper.SeedRolesAndUsersAsync(app.Services).GetAwaiter().GetResult();
}
catch
{
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "TeamIndia.TalentFlow.API v1");
        c.RoutePrefix = "swagger";
    });
}
app.UseGlobalExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
