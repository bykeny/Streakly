# Streakly - Habit & Goal Tracker

Streakly is a modern web application built with ASP.NET Core MVC that helps users build better habits and achieve their goals through consistent tracking and meaningful insights.

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
  - ASP.NET Core MVC (.NET 9)
  - Entity Framework Core
  - ASP.NET Core Identity
  - C# 13.0

- **Frontend**
  - Bootstrap 5
  - JavaScript/jQuery
  - SVG Icons
  - Custom CSS

- **Database**
  - SQL Server
  - Entity Framework Core Migrations

## 📋 Prerequisites

- .NET 9 SDK
- SQL Server (Local or Express)
- Visual Studio 2022 or later

## 🚀 Getting Started

1. **Clone the repository**

`git clone https://github.com/bykeny/Streakly.git`

`cd HabitGoalTrackerApp`

3. **Update database connection string**
- Open `appsettings.json`
- Update the `DefaultConnection` string to your SQL Server instance

3. **Apply database migrations**

`dotnet ef database update`

4. **Run the application**

`dotnet run`

5. **Access the application**
- Open your browser to `https://localhost:7083`
- Register a new account to get started

## 🔧 Configuration

The application supports various configuration options through `appsettings.json`:

- Database connection
- Authentication settings
- Email service configuration
- Logging preferences

## 🎯 Future Roadmap

- [ ] Mobile app integration
- [ ] Social features and habit sharing
- [ ] Advanced analytics and reporting
- [ ] API endpoints for third-party integration
- [ ] Email notifications and reminders
- [ ] Data export/import functionality

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the Project
2. Create your Feature Branch (`git checkout -b feature/AmazingFeature`)
3. Commit your Changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the Branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📧 Contact

Kanan Ramazanov - kenanramaznov@gmail.com
