using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using TeamIndia.TalentFlow.API.Extensions;
using TeamIndia.TalentFlow.Application.ApplicationSettings;
using TeamIndia.TalentFlow.Application.Interfaces;
using TeamIndia.TalentFlow.Domain.Entities;
using TeamIndia.TalentFlow.Infrastructure.DbContext;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TeamIndia.TalentFlow.API", Version = "v1" });
    c.MapType<IFormFile>(() => new OpenApiSchema
    {
        Type = "string",
        Format = "binary"
    });
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = 524288000;
});

builder.WebHost.ConfigureKestrel(options =>
{
    options.Limits.MaxRequestBodySize = long.MaxValue;
});

builder.Services.Configure<FormOptions>(options =>
{
    options.MultipartBodyLengthLimit = long.MaxValue;
    options.ValueLengthLimit = int.MaxValue;
    options.MultipartHeadersLengthLimit = int.MaxValue;
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

// Configuration bindings

builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("Jwt"));
builder.Services.AddSingleton(resolver => resolver.GetRequiredService<IOptions<JwtSettings>>().Value);
builder.Services.Configure<SeedAdminSettings>(builder.Configuration.GetSection("SeedAdmin"));
builder.Services.Configure<BrevoSettings>(builder.Configuration.GetSection("BrevoSettings"));
builder.Services.Configure<CloudinarySettings>(builder.Configuration.GetSection("CloudinarySettings"));
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));
builder.Services.Configure<FrontendSettings>(builder.Configuration.GetSection("Frontend"));
builder.Services.Configure<FormOptions>(options => { options.MultipartBodyLengthLimit = 52428800; });


// Service registrations
builder.Services.AddMemoryCache();
builder.Services.AddScoped<ITokenService, TeamIndia.TalentFlow.Application.Services.TokenService>();
builder.Services.AddScoped<IOtpService, TeamIndia.TalentFlow.Application.Services.OtpService>();
builder.Services.AddScoped<IAdminService, TeamIndia.TalentFlow.Application.Services.AdminService>();
builder.Services.AddScoped<IAuthService, TeamIndia.TalentFlow.Application.Services.AuthService>();
builder.Services.AddScoped<ICloudinaryService, TeamIndia.TalentFlow.Application.Services.CloudinaryService>();
builder.Services.AddScoped<IEmailService, TeamIndia.TalentFlow.Application.Services.EmailService>();
builder.Services.AddScoped<IProfileService, TeamIndia.TalentFlow.Application.Services.ProfileService>();
builder.Services.AddScoped<IOnboardingService, TeamIndia.TalentFlow.Application.Services.OnboardingService>();
builder.Services.AddScoped<ICourseServices, TeamIndia.TalentFlow.Application.Services.CourseServices>();


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
builder.Services.AddScoped<IUserRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.UserRepository>();
builder.Services.AddScoped<IRoleRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.RoleRepository>();
builder.Services.AddScoped<IOnboardingRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.OnboardingRepository>();
builder.Services.AddScoped<ITeamRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.TeamRepository>();
builder.Services.AddAuthorization();
builder.Services.AddScoped<ICourseRepository, TeamIndia.TalentFlow.Infrastructure.Repositories.CourseRepository>();

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
