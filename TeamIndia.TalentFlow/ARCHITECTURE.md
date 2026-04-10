# Architecture & Design Notes — TeamIndia.TalentFlow Backend

This document explains the backend flows, edge-case handling, assumptions, and scalability considerations for the TeamIndia.TalentFlow project.

## 1) Flow explanations (step‑by‑step)

### Signup & Verify
- Frontend calls `POST /api/auth/register` (AuthController -> `AuthService.RegisterAsync`).
  - Service checks duplicates via `IUserRepository.FindByEmailAsync` and creates an `ApplicationUser`.
  - An OTP may be generated and stored via onboarding flow (for demo this is returned when enabled).
  - Passwords are handled by Identity in this project; follow Identity configuration in `Program.cs`.

### Course completion -> Certificate
- Progress is tracked via `IProgressRepository` and `IProgressService`.
- When a user has completed all lessons in a course the certificate flow (`CertificateService.GenerateCertificateAsync`) will:
  - Validate completion via `_progressRepo`.
  - Generate a friendly certificate code (helper: `CertificateCodeHelper.GenerateFriendlyCertificateCode`) that starts with `TF-`.
  - Render a styled HTML certificate using `TeamIndia.TalentFlow.API/Resources/certificate_design.html`.
  - Generate a single-page A4 landscape PDF using PuppeteerSharp and attach it to an email. The attachment filename is `{SanitizedUserName}.pdf`.
  - Send a simple email body (template `TeamIndia.TalentFlow.API/Resources/Emails/certificate.html`) with the PDF attached; the email body does not include the full certificate HTML.

### Assignments list for users
- `AssignmentRepository` exposes a paged API (`GetAssignmentsForUserPagedAsync`) with filter and status (submitted, pending, overdue).
- `AssignmentService` maps domain entities to `AssignmentResponseDto` and computes `AssignmentStatus` (enum) per user.

### Admin flows
- Admin endpoints are grouped under `AdminController` (mentor approval) and `UsersController` exposes user lists (admin-only) and public single-user details.
- Admin checks in many endpoints still use simple role verification via `IUserRepository.GetRolesAsync` or `UserManager` role checks. Replace with proper policy/auth when moving to full identity enforcement.

---

## 2) Edge-case handling (where in code / how handled)

- Duplicate email: handled by user registration checks in `AuthService`/`UserService`.
- Incomplete course: `CertificateService.GenerateCertificateAsync` checks lesson counts and returns 400 if not all lessons are completed.
- Serialization cycles: controllers/services do not return EF entities directly. DTOs (`ProfileDto`, `OnboardingDto`, `UserFullResponseDto`) are used to avoid object-cycle serialization errors.

### Exception handling
- Controllers use the global exception handler middleware to return friendly 4xx/5xx responses. Services return `BaseResponse<T>` that controllers translate to appropriate status codes.

## 3) Assumptions

- The project uses ASP.NET Core Identity for user management in this codebase; however some endpoints perform role checks by `userId` for simplicity in demo flows.
- Certificate friendly code is derived deterministically from a GUID (`TF-XXXX-XXXX`) — the GUID remains the canonical `Certificate.CertificateId` in the DB.
- The email body is intentionally light-weight; the certificate HTML is attached as a PDF.

---

## 4) Scalability & evolution (100 → 10,000+ users)

- Database
  - Move from SQLite to a server RDBMS (Postgres) for concurrency and scale.

- Caching & pagination

- Asynchronous processing

- Security & identity
  - Replace plain-text password storage with ASP.NET Core Identity or a secure hasher and add an authentication system (JWT or cookie-based) for real role enforcement.

---

## 5) Where to find code that implements the flows

- Controllers: `TeamIndia.TalentFlow.API/Controllers/*` (AuthController, UsersController, AdminController, CoursesController, ProfileController, AssignmentsController, CertificatesController)
- Services: `TeamIndia.TalentFlow.Application/Services/*` (AuthService, UserService, CertificateService, AssignmentService, OnboardingService)
- Repositories: `TeamIndia.TalentFlow.Infrastructure/Repositories/*` (UserRepository, AssignmentRepository, OnboardingRepository, CertificateRepository)
- DTOs: `TeamIndia.TalentFlow.Application/Dtos/*` (Response and Request DTOs)
- Certificate templates: `TeamIndia.TalentFlow.API/Resources/certificate_design.html` and `Resources/Emails/certificate.html`

---