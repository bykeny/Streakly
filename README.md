# Streakly - Habit & Goal Tracker

Streakly is a modern web application built with ASP.NET Core MVC that helps users build better habits and achieve their goals through consistent tracking, meaningful insights, and AI-powered analytics.

<img width="1919" height="954" alt="Screenshot_62" src="https://github.com/user-attachments/assets/316b94dd-2d6a-4045-9e42-2ababdf3305a" />

## 🌟 Features

### Habit Tracking
- Create and manage daily, weekday, weekend, or custom scheduled habits
- Track habit completion with an intuitive interface
- Visualize streaks and progress with interactive calendars
- Customizable habit icons and colors
- Progress heatmaps and statistics

### Goal Management
- Set measurable goals with target values and deadlines
- Track progress with detailed logging
- Multiple goal categories (Personal, Health, Career, etc.)
- Progress visualization and analytics
- Goal completion tracking and milestones

### Dashboard & Analytics
- Overview of daily habits and active goals
- Completion rates and streak tracking
- Weekly and monthly progress views
- Visual progress indicators and statistics
- **AI-Powered Insights**: ML.NET-powered weekly insights analyzing habit patterns, goal progress, and mood correlations
- Personalized recommendations and encouragement messages

### Journal
- Daily reflection entries with mood tracking
- Tag-based organization
- Search and filter capabilities
- Mood analysis and trends

### User Experience
- Clean, modern Bootstrap UI
- Dark/Light theme support
- Responsive design for mobile and desktop
- Customizable user preferences

## 🛠️ Technologies

- **Backend**
  - ASP.NET Core MVC (.NET 10)
  - Entity Framework Core
  - ASP.NET Core Identity (Custom MVC Authentication)
  - ML.NET (for AI-powered insights)
  - C# 13.0

- **Frontend**
  - Bootstrap 5
  - JavaScript/jQuery
  - SVG Icons
  - Custom CSS

- **Database**
  - SQL Server
  - Entity Framework Core Migrations

## 🔐 Authentication

The application uses a custom MVC-based authentication system built on ASP.NET Core Identity:

- **Custom AuthController** replaces default Identity Razor Pages
- Routes: `/Auth/Login`, `/Auth/Register`, `/Auth/Logout`, `/Auth/ForgotPassword`
- Features: password reset, account lockout, remember me, secure cookies
- Email service integration ready (currently logs to console in development)
- For production: configure email service in `Services/Implementation/EmailSender.cs`

## 📋 Prerequisites

- .NET 10 SDK (for local non-Docker development)
- Docker Desktop (for containerized development)
- SQL Server (Local/Express or Dockerized via `docker-compose`)
- Visual Studio 2022 or later (optional but recommended)

## 🚀 Getting Started (Local Development with Docker)

This project includes a Docker setup that runs the web app and a SQL Server database together using `docker-compose`.

### 1. Clone the repository

```bash
git clone https://github.com/bykeny/Streakly.git
cd HabitGoalTrackerApp
```

### 2. Create a `.env` file

In the project root (`HabitGoalTrackerApp`), create a file named `.env`:

```env
MSSQL_SA_PASSWORD=YourStrong!Passw0rd123
```

> Use a strong password and **do not commit** the `.env` file to source control.

### 3. Start the app and database with Docker

From the project root:

```bash
docker compose up --build
```

This will:

- Build the `HabitGoalTrackerApp` image using the `Dockerfile`.
- Start the `web` container (ASP.NET Core app) and the `db` container (SQL Server 2022).
- Configure the app to connect to SQL Server via the `ConnectionStrings__HabitGoalConnection` environment variable.

### 4. Access the application

- Open your browser and go to: `http://localhost:5032`
- Register a new account to get started.

### 5. Stopping the containers

Press `Ctrl+C` in the terminal that is running `docker compose up`, or run:

```bash
docker compose down
```

## 🚀 Getting Started (Local Development without Docker)

You can also run the app directly with the .NET SDK and your own SQL Server instance.

1. **Clone the repository** (same as above).
2. **Configure the connection string**:
  - Open `appsettings.json`.
  - Update `ConnectionStrings:HabitGoalConnection` to point to your SQL Server.
  - Alternatively, use user-secrets:

    ```bash
    dotnet user-secrets set "ConnectionStrings:HabitGoalConnection" "Server=(localdb)\\MSSQLLocalDB;Database=HabitGoalDB;Trusted_Connection=True;"
    ```

3. **Apply database migrations** (optional, if not applied yet):

  ```bash
  dotnet ef database update
  ```

4. **Run the application**:

  ```bash
  dotnet run
  ```

5. **Access the application**:
  - Open your browser at `https://localhost:5032` or the URL printed in the console.

## 🔧 Configuration

The application supports various configuration options through `appsettings.json`:

- Database connection
- Authentication settings
- Email service configuration
- Logging preferences

## 🔄 CI/CD with GitHub Actions & Docker Hub

This repository includes a GitHub Actions workflow (`.github/workflows/ci-docker.yml`) that builds, tests, and containerizes the app, then pushes images to Docker Hub.

### Workflow overview

On pushes to `main` or `containers` branches:

1. **Build & test** (`build_and_test` job)
  - Checks out the repository.
  - Sets up .NET 10 SDK.
  - Runs `dotnet restore`, `dotnet build`, and `dotnet test` in Release configuration.

2. **Docker build & push** (`docker_build_and_push` job)
  - Runs only if the build & tests succeed.
  - Logs in to Docker Hub using repository secrets:
    - `DOCKERHUB_USERNAME`
    - `DOCKERHUB_PASSWORD` (access token with read/write scope).
  - Builds the Docker image using the root `Dockerfile`.
  - Pushes the image to Docker Hub (`bykeny/habit-goal-tracker`) with tags:
    - `latest`
    - `${{ github.sha }}` (commit SHA)
    - `v0.1.${{ github.run_number }}` (simple semantic-style build tag)

This makes it easy to track exactly which build is running from the tag.

## 🐳 Using the Docker Hub Image

Once the CI pipeline has pushed an image, you can pull and run it directly from Docker Hub.

### 1. Pull the image

```bash
docker pull bykeny/habit-goal-tracker:latest
```

You can also use a specific version tag, for example:

```bash
docker pull bykeny/habit-goal-tracker:v0.1.10
```

### 2. Run the app container

You still need a SQL Server instance. For quick testing, you can:

1. Start a SQL Server container:

  ```bash
  docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd123" -p 1433:1433 -d mcr.microsoft.com/mssql/server:2022-latest
  ```

2. Run the app, passing the connection string via environment variable:

  ```bash
  docker run -p 5032:8080 \
    -e "ConnectionStrings__HabitGoalConnection=Server=host.docker.internal,1433;Database=HabitGoalDB;User Id=sa;Password=YourStrong!Passw0rd123;TrustServerCertificate=True;Encrypt=False" \
    bykeny/habit-goal-tracker:latest
  ```

Then open `http://localhost:5032` in your browser.

## 🎯 Future Roadmap

- [x] AI-powered insights with ML.NET
- [ ] Mobile app integration
- [ ] Social features and habit sharing
- [ ] Advanced analytics and reporting
- [ ] API endpoints for third-party integration
- [ ] Email notifications and reminders
- [ ] Data export/import functionality
- [ ] Gamification and achievement system
- [ ] Push notifications and reminders

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request


## 📧 Contact

Kanan Ramazanov - kenanramaznov@gmail.com

---

**Built with ❤️ using ASP.NET Core and ML.NET**
