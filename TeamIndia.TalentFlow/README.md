# TeamIndia.TalentFlow - Backend

This repository contains the backend implementation for TeamIndia.TalentFlow (ASP.NET Core / .NET 10). It exposes APIs for authentication, courses, progress, assignments, certificates, user profiles and admin flows.

Projects
- `TeamIndia.TalentFlow.API/` - main web API project (startup, controllers)
- `TeamIndia.TalentFlow.Application/` - application services, DTOs, interfaces
- `TeamIndia.TalentFlow.Domain/` - EF Core entities
- `TeamIndia.TalentFlow.Infrastructure/` - Repositories and DbContext

Quick start

1. Build

```sh
dotnet build
```

Clone the repository
--------------------
To clone this repository and switch to the feature branch used in the workspace run:

```sh
git clone https://github.com/TruemindLMS/TIms-Backend.git
cd TIms-Backend
git fetch --all
git checkout to Main branch
```

If you prefer SSH:

```sh
git clone git@github.com:TruemindLMS/TIms-Backend.git
cd TIms-Backend
git fetch --all
git checkout to Main branch
```

2. (If you changed entities) Apply EF Core migrations

- Install the EF tool if required: `dotnet tool install --global dotnet-ef`
- From the solution root add migration and update the database (uses the Infrastructure project and API as startup):

```sh
dotnet ef migrations add Init --project TeamIndia.TalentFlow.Infrastructure --startup-project TeamIndia.TalentFlow.API
dotnet ef database update --project TeamIndia.TalentFlow.Infrastructure --startup-project TeamIndia.TalentFlow.API
```

By default the project reads the database connection from the `DATABASE_URL` environment variable or `appsettings.json` in the API project.

3. Run

```sh
dotnet run --project TeamIndia.TalentFlow.API
```

Swagger UI can be accessed at https://localhost:{PORT}/swagger/index.html when it is enabled. After logging in, obtain your JWT token and use the Authorize button in Swagger to paste and apply the token for authenticated requests.

OR 

Can be access online https://tims-backend-11dz.onrender.com/swagger/index.html

Certificate generation
- The service generates PDF certificates using PuppeteerSharp. On first run PuppeteerSharp will download a compatible Chromium binary. Make sure the host environment allows the download and execution of headless Chromium.

Environment variables and configuration
- `DATABASE_URL` - (optional) database connection string. If not set the API reads the configured `DefaultConnection`.
- `ENABLE_SWAGGER` - set to `true` to enable swagger in non-development environments.
- JWT settings are under the `Jwt` section in `appsettings.json` (or environment variables) — configure `Key`, `Issuer`, `Audience` for authentication to work.

API endpoints (summary)
- Auth
  - `POST /api/auth/register` - Register a new user
  - `POST /api/auth/login` - Login and receive JWT
  - `POST /api/auth/forgot-password` - Request password reset

- Users
  - `GET /api/users/{id}` - Get a user's public details (profile, onboarding, roles)
  - `GET /api/users` - Admin only: list users (supports paging query string `page` & `pageSize`)

- Profile & Onboarding
  - `GET/POST` endpoints under `/api/profile` and `/api/onboarding` manage user profile and onboarding data

- Courses / Progress
  - Endpoints under `/api/courses` and `/api/progress` allow listing courses, fetching modules/lessons, and tracking completion

- Assignments
  - Assignment endpoints support creating assignments, submitting and fetching user-specific paged lists (with status filters)

- Certificates
  - `POST /api/certificates/{courseId}/generate` - Generate and email certificate PDF to user (attachment named `{UserName}.pdf`)
  - `GET /api/certificates/{courseId}/{userId}` - Get certificate metadata

Notes, conventions and caveats
- The backend is implemented as a demo and may include simplifications. Review authentication, authorization and input validation before using in production.
- When generating certificates the HTML template used for the PDF is located at `TeamIndia.TalentFlow.API/Resources/certificate_design.html`. The email body uses `TeamIndia.TalentFlow.API/Resources/Emails/certificate.html` and the PDF is attached to the email.
- The certificate PDF filename is the sanitized user full name (e.g. `Stanley_Ikemefuna.pdf`). The friendly certificate code (prefixed `TF-`) is embedded into the PDF and visible on the certificate.

Troubleshooting
- If the PDF attachment is producing multiple pages, ensure the API was restarted after changes to the template and that PuppeteerSharp successfully downloaded Chromium. If necessary increase available memory or adjust the CSS in `certificate_design.html`.
- If migrations fail, confirm the EF tools are installed and you are running the commands from the solution root.

Contact
- For updates to endpoints or run instructions, open an issue or contact the maintainer.

Seed data
---------
The application includes runtime seeders that populate initial data on first run (roles, an initial admin user, sample courses and teams). Seeding is performed during application startup by the helpers in `TeamIndia.TalentFlow.API.Helpers` (for example `SeedDataHelper`, `DataSeeder`, `TeamDataSeeder`).

You can inspect the console/log output when the app starts to see seeding messages. To disable or change the seeded data modify or remove the seeding calls in `Program.cs`.

Local configuration (appsettings.json)
------------------------------------
When running the API locally create an `appsettings.Development.json` (or edit `appsettings.json`) with the required configuration. Do NOT commit real credentials — use environment variables or a secrets store for production.

Example `appsettings.Development.json` (replace placeholder values):

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=talentflow_dev;Username=postgres;Password=your_db_password"
  },
  "Jwt": {
    "Key": "Replace_With_A_Strong_Secret_Key_At_Least_32_Chars",
    "Issuer": "TalentFlowApi",
    "Audience": "TalentFlowClients",
    "ExpiryMinutes": 60
  },
  "BrevoSettings": {
    "ApiKey": "BREVO_API_KEY_EXAMPLE",
    "FromEmail": "no-reply@example.com"
  },
  "CloudinarySettings": {
    "CloudName": "your_cloud_name",
    "ApiKey": "your_cloudinary_key",
    "ApiSecret": "your_cloudinary_secret"
  },
  "SeedAdmin": {
    "Email": "admin@talentflow.com",
    "Password": "TalentFlow@2026!",
    "FullName": "TalentFlow Admin"
  },
  "SmtpSettings": {
    "Host": "smtp.example.com",
    "Port": 587,
    "Username": "smtp_user",
    "Password": "smtp_password",
    "From": "no-reply@example.com"
  },
  "Frontend": {
    "Url": "http://localhost:3000"
  },
  "EnableSwagger": true
}
```

Notes
- The application will prefer the `DATABASE_URL` environment variable if present. You can set it instead of editing `appsettings`:

```sh
export DATABASE_URL="Host=localhost;Port=5432;Database=talentflow_dev;Username=postgres;Password=your_db_password"
```

- Keep secrets out of source control. Use `dotnet user-secrets` for local development or your cloud provider's secret manager in CI/CD.
