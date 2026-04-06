using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
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

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL")
                      ?? builder.Configuration.GetConnectionString("DefaultConnection");

if (string.IsNullOrEmpty(connectionString))
{
    throw new Exception("Database connection string is not configured.");
}

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

builder.Services.AddIdentity<ApplicationUser, ApplicationRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();


builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSettings>>().Value);
builder.Services.Configure<SeedAdminSettings>(builder.Configuration.GetSection("SeedAdmin"));

// Infrastructure/Application service registrations
builder.Services.AddMemoryCache();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.ITokenService, TeamIndia.TalentFlow.Application.Services.TokenService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IOtpService, TeamIndia.TalentFlow.Application.Services.OtpService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IAdminService, TeamIndia.TalentFlow.Application.Services.AdminService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IAuthService, TeamIndia.TalentFlow.Application.Services.AuthService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IEmailService, TeamIndia.TalentFlow.Application.Services.EmailService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IProfileService, TeamIndia.TalentFlow.Application.Services.ProfileService>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IOnboardingService, TeamIndia.TalentFlow.Application.Services.OnboardingService>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.ICourseServices, TeamIndia.TalentFlow.Application.Services.CourseServices>();


builder.Services.AddCors(options =>
{
    options.AddPolicy("SecurePolicy", policy =>
    {
        policy.WithOrigins(
                "http://localhost:3000",
                "https://talentflow-eight-weld.vercel.app"
            )
            .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
            .WithHeaders("Content-Type", "Authorization");
    });
});
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.ITeamServices, TeamIndia.TalentFlow.Application.Services.TeamServices>();
;

// Repositories
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IFileStorageService>(sp =>
{
    var env = sp.GetRequiredService<IWebHostEnvironment>();
    var logger = sp.GetRequiredService<ILogger<TeamIndia.TalentFlow.Application.Services.LocalFileStorageService>>();
    return new TeamIndia.TalentFlow.Application.Services.LocalFileStorageService(env.WebRootPath, logger);
});
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IUserRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IRoleRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.RoleRepository>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.IOnboardingRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.OnboardingRepository>();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.ITeamRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.TeamRepository>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<TeamIndia.TalentFlow.Application.Interfaces.ICourseRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.CourseRepository>();

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

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

try
{
    TeamIndia.TalentFlow.API.Helpers.SeedDataHelper.SeedRolesAndUsersAsync(app.Services).GetAwaiter().GetResult();

    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // Apply any pending migrations to ensure database schema is up to date
    db.Database.Migrate();
    TeamIndia.TalentFlow.API.Helpers.DataSeeder.SeedCoursesAsync(db).GetAwaiter().GetResult();
}
catch
{
}

// Configure the HTTP request pipeline.
// Enable swagger when in Development OR when ENABLE_SWAGGER is set to true (env or config)
var enableSwagger = app.Environment.IsDevelopment()
                    || builder.Configuration.GetValue<bool>("ENABLE_SWAGGER")
                    || string.Equals(Environment.GetEnvironmentVariable("ENABLE_SWAGGER"), "true", StringComparison.OrdinalIgnoreCase);

if (enableSwagger)
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

// enable CORS (use the registered policy name)
app.UseCors("SecurePolicy");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
