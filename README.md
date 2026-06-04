# Memorio

Fullstack flashcard learning app built with ASP.NET Core, React, TypeScript and PostgreSQL. Memorio helps users create decks, manage flashcards and study with an FSRS-based spaced repetition scheduler.

> Portfolio project focused on production-style .NET architecture, authentication, persistence, integration tests and a usable React frontend.

## Highlights

- Modular ASP.NET Core API split into Users, Flashcards and Learning modules.
- JWT authentication with refresh tokens stored in secure HTTP-only cookies.
- Deck and card management with pagination, validation and owner-based access control.
- Learning sessions powered by FSRS spaced repetition scheduling.
- React + TypeScript frontend with protected routes, auth state restoration and i18n.
- PostgreSQL persistence with EF Core migrations.
- Unit and integration tests using xUnit, Testcontainers and ASP.NET Core test host.
- Docker Compose setup for local infrastructure.

## Tech Stack

### Backend

- .NET 10 / ASP.NET Core
- Entity Framework Core + PostgreSQL
- MediatR for CQRS-style command/query handlers
- FluentValidation
- ASP.NET Core Identity and JWT bearer authentication
- Serilog
- FsrsSharp
- Sieve pagination

### Frontend

- React
- TypeScript
- Vite
- React Router
- React Hook Form + Zod
- i18next
- Tailwind CSS
- Axios

### Testing and Infrastructure

- xUnit
- AwesomeAssertions
- Testcontainers for PostgreSQL-backed integration tests
- Docker Compose
- PostgreSQL, Redis and MinIO services
- npm workspaces + Turborepo

## Architecture

The backend is organized as a modular monolith. Each feature area owns its domain model, persistence and API endpoints, while shared behaviors live in `Memorio.Shared`.

```text
src/
  Memorio.API/          API host, middleware, Swagger, composition root
  Memorio.Users/        registration, login, JWT issuing, refresh tokens
  Memorio.Flashcards/   decks, cards, CRUD endpoints, pagination
  Memorio.Learning/     study sessions, reviews, FSRS scheduling, stats
  Memorio.Shared/       domain primitives, validation behavior, results, user context

apps/
  web/                  React + TypeScript frontend
  shared/               shared frontend API/types package

tests/
  *.UnitTests/          domain/application tests
  *.IntegrationTests/   endpoint tests with real PostgreSQL containers
```

## Core Features

### Authentication

- User registration and login.
- Short-lived access tokens.
- Refresh token rotation through HTTP-only cookies.
- Authenticated `/me` endpoint.
- Integration tests covering invalid credentials, missing tokens and refresh-token reuse.

### Flashcards

- Create, update, delete and list decks.
- Create, update, delete and list cards.
- Paginated deck/card queries.
- Per-user data isolation.
- Validation through FluentValidation.

### Learning

- Start study sessions from due cards.
- Review cards with Again, Hard, Good and Easy ratings.
- Schedule future reviews with FSRS.
- Track card progress and study session history.
- User learning statistics.

### Frontend

- Login and registration flow.
- Protected dashboard and deck pages.
- Deck and card management UI.
- Study session screen with rating buttons and summary.
- English and Polish translations.

## Getting Started

### Prerequisites

- .NET 10 SDK
- Node.js and npm
- Docker Desktop

### 1. Configure environment

Create a local `.env` file in the repository root:

```env
POSTGRES_USER=memorio
POSTGRES_PASSWORD=change_me_in_production
POSTGRES_DB=memorio_db

REDIS_PASSWORD=change_me_in_production

MINIO_ROOT_USER=memorio
MINIO_ROOT_PASSWORD=change_me_in_production
STORAGE_BUCKET_NAME=memorio

JWT_SECRET=your_super_secret_key_min_32_characters_long
JWT_ISSUER=memorio-api
JWT_AUDIENCE=memorio-client

CLAUDE_API_KEY=sk-ant-...
ASPNETCORE_ENVIRONMENT=Development
```

### 2. Start infrastructure

```bash
docker compose up -d
```

### 3. Run the API

```bash
dotnet run --project src/Memorio.API/Memorio.API.csproj
```

The API applies EF Core migrations on startup in development. Swagger is available from the ASP.NET Core development URL.

### 4. Run the frontend

```bash
npm install
npm run dev --workspace web
```

The React app runs on Vite, usually at `http://localhost:5173`.

## Tests

Run the full .NET test suite:

```bash
dotnet test Memorio.slnx
```

Run the frontend build:

```bash
npm run build --workspace web
```

## API Areas

- `POST /api/v1/auth/register`
- `POST /api/v1/auth/login`
- `POST /api/v1/auth/refresh`
- `GET /api/v1/auth/me`
- `GET /api/v1/decks`
- `POST /api/v1/decks`
- `GET /api/v1/decks/{deckId}`
- `PUT /api/v1/decks/{deckId}`
- `DELETE /api/v1/decks/{deckId}`
- `GET /api/v1/decks/{deckId}/cards`
- `POST /api/v1/decks/{deckId}/cards`
- `GET /api/v1/decks/{deckId}/reviews/due`
- `POST /api/v1/decks/{deckId}/sessions/start`
- `GET /api/v1/sessions`
- `POST /api/v1/sessions/{sessionId}/review`
- `POST /api/v1/sessions/{sessionId}/complete`
- `GET /api/v1/stats`

## What This Project Demonstrates

- Clean separation between API, application, domain and infrastructure concerns.
- Practical CQRS with MediatR without over-engineering the codebase.
- Secure token handling with refresh-token rotation.
- Real integration tests instead of only mocked controller tests.
- A fullstack workflow where backend contracts are consumed by a typed React client.
- Product thinking around a real learning workflow, not just CRUD screens.

## Roadmap

- AI-assisted flashcard generation from notes or documents.
- Public demo deployment.
- GitHub Actions pipeline for backend tests and frontend build.
- Screenshots and short demo video in this README.
- Import/export for decks.
- More learning analytics and streak tracking.
