# Habit & Goal Tracker App - Single Docker Image
# Usage:
#   docker run -p 8080:8080 bykeny/habit-goal-tracker
#   (Uses in-memory database if DATABASE_URL not provided)
#
#   docker run -p 8080:8080 -e DATABASE_URL="Server=...;Database=...;..." bykeny/habit-goal-tracker
#   (Uses SQL Server with provided connection string)

# Build stage
FROM mcr.microsoft.com/dotnet/sdk:10.0 AS build
WORKDIR /src

# Copy project file and restore dependencies
COPY HabitGoalTrackerApp.csproj ./
RUN dotnet restore

# Copy the rest of the source and publish
COPY . .
RUN dotnet publish -c Release -o /app/publish

# Runtime stage (no SQL Server needed - app only)
FROM mcr.microsoft.com/dotnet/aspnet:10.0 AS final
WORKDIR /app

COPY --from=build /app/publish .

# Configure ASP.NET Core
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

EXPOSE 8080

ENTRYPOINT ["dotnet", "HabitGoalTrackerApp.dll"]
