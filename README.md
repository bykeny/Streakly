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
- Routes: `/auth/login`, `/auth/register`, `/auth/logout`, `/auth/forgot-password`
- Features: password reset, account lockout, remember me, secure cookies
- **Email confirmation required** for new registrations
- SMTP email service with MailKit (configurable via environment variables)

## 📋 Prerequisites

- .NET 10 SDK (for local non-Docker development)
- Docker Desktop (for containerized development)
- SQL Server (Local/Express or Dockerized via `docker-compose`) - optional, app can run with in-memory database
- Visual Studio 2022 or later (optional but recommended)

## 🚀 Quick Start with Docker (No Database Required)

The simplest way to try the app - runs with an in-memory database (data resets on restart):

```bash
docker run -p 8080:8080 bykeny/habit-goal-tracker
```

Then open `http://localhost:8080` in your browser.

## 🚀 Getting Started (Local Development with Docker Compose)

For persistent data storage, use `docker-compose` which runs the app with a SQL Server database.

### 1. Clone the repository

```bash
git clone https://github.com/bykeny/Streakly.git
cd Streakly
```

### 2. Create a `.env` file

In the project root (`Streakly`), create a file named `.env`:

```env
MSSQL_SA_PASSWORD=YourStrong!Passw0rd123

# Email settings (optional - if not set, emails are logged to console)
SMTP_HOST=smtp.gmail.com
SMTP_PORT=587
SMTP_USERNAME=your-email@gmail.com
SMTP_PASSWORD=your-gmail-app-password
SMTP_FROM_EMAIL=your-email@gmail.com
SMTP_FROM_NAME=Streakly
SMTP_USE_SSL=true
```

> ⚠️ **Important**: 
> - Use a strong password and **do not commit** the `.env` file to source control.
> - For Gmail, you need an [App Password](https://myaccount.google.com/apppasswords) (requires 2FA enabled).

### 3. Start the app and database with Docker

From the project root:

```bash
docker compose up --build
```

This will:

- Build the `bykeny/habit-goal-tracker` image using the `Dockerfile`.
- Start the `db` container (SQL Server 2022) with a healthcheck.
- Wait for SQL Server to be healthy, then start the `web` container (ASP.NET Core app).
- Configure the app to connect to SQL Server via the `DATABASE_URL` environment variable.

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

### Option 1: Quick Start (In-Memory Database)

For quick testing without any database setup:

```bash
docker run -p 8080:8080 bykeny/habit-goal-tracker
```

> ⚠️ **Note**: Data is stored in-memory and will be lost when the container stops.

### Option 2: With External SQL Server (Persistent Data)

Connect to an existing SQL Server instance using the `DATABASE_URL` environment variable:

```bash
docker run -p 8080:8080 \
  -e "DATABASE_URL=Server=your-server,1433;Database=HabitGoalDB;User Id=sa;Password=YourPassword;TrustServerCertificate=True;Encrypt=False" \
  bykeny/habit-goal-tracker
```

### Option 3: With Docker Compose (Recommended for Development)

Use `docker-compose.yml` to run both the app and SQL Server together:

```bash
# Set the SQL Server password
export MSSQL_SA_PASSWORD="YourStrong!Passw0rd123"  # Linux/Mac
# or
$env:MSSQL_SA_PASSWORD = "YourStrong!Passw0rd123"  # Windows PowerShell

# Start everything
docker compose up
```

Then open `http://localhost:5032` in your browser.

### Pulling Specific Versions

```bash
# Latest version
docker pull bykeny/habit-goal-tracker:latest

# Specific version
docker pull bykeny/habit-goal-tracker:v0.1.10
```

## ☁️ Azure Deployment with Terraform

For production deployment, this application can be deployed to Azure Container Apps using Terraform infrastructure-as-code. This provides a fully managed, serverless container hosting platform with built-in monitoring and scalability.

### Architecture Overview

The Terraform configuration in the `terraform/` directory creates the following Azure resources:

```
┌─────────────────────────────────────────────────────────┐
│                 Azure Resource Group                    │
│                   (rg-streakly)                         │
│                                                         │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Log Analytics Workspace (law-streakly)            │ │
│  │  • Logs and metrics collection                     │ │
│  │  • 30-day retention                                │ │
│  └────────────────────────────────────────────────────┘ │
│                        ↓                                │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Application Insights (appi-streakly)              │ │
│  │  • APM and monitoring                              │ │
│  │  • Performance tracking                            │ │
│  └────────────────────────────────────────────────────┘ │
│                        ↓                                │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Container App Environment (env-streakly)          │ │
│  │  • Managed Kubernetes-like environment             │ │
│  └────────────────────────────────────────────────────┘ │
│                        ↓                                │
│  ┌────────────────────────────────────────────────────┐ │
│  │  Container App (streakly-app)                      │ │
│  │  • Docker image: bykeny/habit-goal-tracker:latest  │ │
│  │  • CPU: 0.25 cores, Memory: 0.5 Gi                 │ │
│  │  • Scale: 0-1 replicas (scale-to-zero)             │ │
│  │  • External ingress on port 8080                   │ │
│  │  • SMTP/Email configuration (secrets)              │ │
│  └────────────────────────────────────────────────────┘ │
└─────────────────────────────────────────────────────────┘
```

**Resources Created:**
1. **Resource Group** - Container for all Azure resources
2. **Log Analytics Workspace** - Centralized logging (30-day retention)
3. **Application Insights** - Application performance monitoring and telemetry
4. **Container App Environment** - Managed environment for running containers
5. **Container App** - The Streakly application with external HTTPS endpoint

### Prerequisites

Before deploying to Azure, ensure you have:

1. **Azure Subscription** - Active Azure account ([Free trial available](https://azure.microsoft.com/free/))
2. **Terraform CLI** - Version >= 1.5.0 ([Download](https://www.terraform.io/downloads))
3. **Azure CLI** - For authentication ([Install guide](https://learn.microsoft.com/cli/azure/install-azure-cli))
4. **SMTP Credentials** - Gmail account with app password for email notifications
5. **Azure Service Principal** (for GitHub Actions) - See [Azure docs](https://learn.microsoft.com/azure/developer/github/connect-from-azure)

### Local Deployment with Terraform

#### 1. Authenticate with Azure

```bash
# Login to Azure
az login

# Set your subscription (if you have multiple)
az account set --subscription "YOUR_SUBSCRIPTION_ID"

# Verify authentication
az account show
```

#### 2. Configure Variables

Create a `terraform/terraform.tfvars` file with your configuration:

```hcl
# Azure Configuration
location            = "polandcentral"  # or your preferred region
resource_group_name = "rg-streakly"
containerapp_name   = "streakly-app"
environment_name    = "env-streakly"

# Docker Image
image_name = "bykeny/habit-goal-tracker:latest"

# SMTP Configuration (Required for email notifications)
smtp_host        = "smtp.gmail.com"
smtp_port        = "587"
smtp_username    = "your-email@gmail.com"
smtp_password    = "your-gmail-app-password"  # Use Gmail App Password
smtp_from_email  = "your-email@gmail.com"
smtp_from_name   = "Streakly"
smtp_use_ssl     = "true"

# Application Insights
appinsights_name = "appi-streakly"
```

> ⚠️ **Security Warning**: 
> - **Never commit `terraform.tfvars` to source control** - it contains sensitive credentials
> - Add `terraform.tfvars` to `.gitignore`
> - For production, use Azure Key Vault or GitHub Secrets instead of plain text passwords
> - Use [Gmail App Passwords](https://myaccount.google.com/apppasswords) (requires 2FA enabled)

#### 3. Initialize Terraform

```bash
cd terraform
terraform init
```

This downloads the Azure provider and prepares the working directory.

#### 4. Plan Infrastructure Changes

```bash
terraform plan
```

Review the proposed changes. Terraform will show you what resources will be created.

#### 5. Apply Infrastructure

```bash
terraform apply
```

Type `yes` when prompted to create the resources. This process takes 3-5 minutes.

#### 6. Get the Application URL

After successful deployment:

```bash
terraform output container_app_url
```

Open the URL in your browser to access your deployed application.

#### 7. Destroy Infrastructure (when needed)

```bash
terraform destroy
```

Type `yes` to delete all Azure resources and stop incurring costs.

### GitHub Actions Automation

The repository includes three automated Terraform workflows:

#### 1. Terraform Plan (`.github/workflows/terraform-plan.yml`)

**Trigger**: Pull requests to `main` branch

**Purpose**: Validates Terraform configuration and shows infrastructure changes before merging

**Actions**:
- Checks out code
- Authenticates with Azure using OIDC
- Sets up Terraform
- Runs `terraform fmt -check` (validates formatting)
- Runs `terraform init`
- Runs `terraform validate` (checks syntax)
- Runs `terraform plan` (shows proposed changes)
- Posts plan results as PR comment with status table
- Uploads plan artifact

**Required Secrets**:
- `AZURE_CLIENT_ID` - Service Principal Client ID
- `AZURE_TENANT_ID` - Azure Tenant ID
- `AZURE_SUBSCRIPTION_ID` - Azure Subscription ID

**Features**:
- ✅ Concurrency control per PR
- ✅ Automated PR comments with plan results
- ✅ Format checking

#### 2. Terraform Apply (`.github/workflows/terraform-apply.yml`)

**Trigger**: Push to `main` branch

**Purpose**: Automatically applies infrastructure changes to Azure

**Actions**:
- Authenticates with Azure using OIDC
- Runs `terraform init`
- Runs `terraform apply -auto-approve` (creates/updates infrastructure)

**Required Secrets**:
- `AZURE_CLIENT_ID` - Service Principal Client ID
- `AZURE_TENANT_ID` - Azure Tenant ID
- `AZURE_SUBSCRIPTION_ID` - Azure Subscription ID
- `SMTP_USERNAME` - SMTP username (passed as `TF_VAR_smtp_username`)
- `SMTP_PASSWORD` - SMTP password (passed as `TF_VAR_smtp_password`)

**Features**:
- ✅ Concurrency control (prevents simultaneous applies)

> **Note**: Variables prefixed with `TF_VAR_` are automatically passed to Terraform as input variables.

#### 3. Deploy to Container Apps (`.github/workflows/deploy-aca.yml`)

**Trigger**: Push to `main` branch

**Purpose**: Updates the Azure Container App with the latest Docker image

**Actions**:
- Authenticates with Azure using OIDC
- Updates container app image to latest version
- Triggers new revision deployment

**Required Secrets**:
- `AZURE_CLIENT_ID` - Service Principal Client ID
- `AZURE_TENANT_ID` - Azure Tenant ID
- `AZURE_SUBSCRIPTION_ID` - Azure Subscription ID

**Features**:
- ✅ OIDC authentication (no stored credentials)
- ✅ Concurrency control

### Setting Up GitHub Actions Secrets

To enable automated deployments with OIDC:

1. **Create Azure Service Principal**:

```bash
az ad sp create-for-rbac --name "github-actions-streakly" \
  --role contributor \
  --scopes /subscriptions/YOUR_SUBSCRIPTION_ID
```

Save the `appId` (Client ID) and `tenant` from the output.

2. **Configure Federated Credentials** (for OIDC):

```bash
# Replace with your values
APP_ID="your-app-id-from-step-1"
REPO_OWNER="bykeny"
REPO_NAME="Streakly"

# Create federated credential
az ad app federated-credential create \
  --id $APP_ID \
  --parameters '{
    "name": "github-actions",
    "issuer": "https://token.actions.githubusercontent.com",
    "subject": "repo:'"$REPO_OWNER"'/'"$REPO_NAME"':ref:refs/heads/main",
    "audiences": ["api://AzureADTokenExchange"]
  }'
```

3. **Add Repository Secrets** (Settings → Secrets and variables → Actions):

| Secret Name | Value | Description |
|-------------|-------|-------------|
| `AZURE_CLIENT_ID` | From Service Principal (appId) | Client ID for OIDC |
| `AZURE_TENANT_ID` | From Service Principal (tenant) | Tenant ID for OIDC |
| `AZURE_SUBSCRIPTION_ID` | Your Azure subscription ID | Subscription ID |
| `SMTP_USERNAME` | Your email address | SMTP username |
| `SMTP_PASSWORD` | Gmail app password | SMTP password |

> **🔒 Security Note**: This setup uses **OIDC (OpenID Connect)** federated credentials instead of storing long-lived secrets. Authentication happens via short-lived tokens - no passwords or keys are stored in GitHub.

### Terraform Outputs

After deployment, Terraform provides the following outputs:

| Output | Description | Usage |
|--------|-------------|-------|
| `container_app_url` | Public HTTPS URL of your application | Access your deployed app |
| `app_insights_url` | Application Insights App ID | For monitoring and diagnostics |

Access outputs:

```bash
# All outputs
terraform output

# Specific output
terraform output container_app_url
```

### Monitoring and Observability

#### Application Insights

- **Access**: Azure Portal → Resource Group → Application Insights (appi-streakly)
- **Features**:
  - Real-time performance monitoring
  - Request/response tracking
  - Failure and exception tracking
  - Custom metrics and events
  - User analytics

#### Log Analytics

- **Access**: Azure Portal → Resource Group → Log Analytics Workspace (law-streakly)
- **Query Logs**: Use Kusto Query Language (KQL) to analyze container logs
- **Retention**: 30 days (configurable in `terraform/loganalytics.tf`)

**Example Log Query**:

```kql
ContainerAppConsoleLogs_CL
| where ContainerAppName_s == "streakly-app"
| order by TimeGenerated desc
| take 100
```

### Cost Estimation

Azure Container Apps pricing is based on:

- **vCPU**: $0.000024/vCPU-second (0.25 vCPU configured)
- **Memory**: $0.000003/GiB-second (0.5 GiB configured)
- **Scale-to-zero**: No charges when app has 0 replicas (no traffic)
- **Log Analytics**: Free tier: 5GB/month, then $2.99/GB
- **Application Insights**: Free tier: 5GB/month, then $2.99/GB

**Estimated Monthly Cost**: ~$5-15 for low to moderate traffic

> 💡 **Tip**: The app is configured with `min_replicas = 0` to enable scale-to-zero, reducing costs during idle periods.

### Important Notes

#### Database Configuration

⚠️ **Current Limitation**: The Azure deployment uses an **in-memory database**. Data is lost when the container restarts or scales to zero.

**For Production Use**, consider:
- Adding Azure SQL Database to `terraform/` configuration
- Updating connection string in Container App environment variables
- Implementing database migrations in the deployment pipeline

#### State Management

The current configuration uses **local state files** (`terraform.tfstate`). For team collaboration or production use:

**Recommended**: Configure Azure Blob Storage backend:

```hcl
# Add to terraform/provider.tf
terraform {
  backend "azurerm" {
    resource_group_name  = "rg-terraform-state"
    storage_account_name = "tfstatestreakly"
    container_name       = "tfstate"
    key                  = "streakly.terraform.tfstate"
  }
}
```

#### Cold Starts

With `min_replicas = 0`, the first request after idle period may experience a cold start (3-5 seconds). To avoid this:

- Set `min_replicas = 1` in `terraform/containerapp.tf`
- Accept slightly higher costs for always-on availability

### Deployment Comparison

| Feature | Docker Compose (Local) | Azure Container Apps (Terraform) |
|---------|------------------------|----------------------------------|
| **Environment** | Local development | Cloud production |
| **Database** | SQL Server container | In-memory (add Azure SQL for persistence) |
| **Scalability** | Single instance | Auto-scaling (0-1 replicas configured) |
| **High Availability** | ❌ No | ✅ Yes (Azure SLA 99.95%) |
| **Monitoring** | Console logs only | Application Insights + Log Analytics |
| **Email Notifications** | Optional | Configured via SMTP secrets |
| **HTTPS/SSL** | ❌ HTTP only | ✅ Automatic HTTPS with Azure |
| **Cost** | Free (local resources) | ~$5-15/month |
| **Deployment Time** | Instant | 3-5 minutes |
| **Best For** | Development & Testing | Production & Staging |

### Troubleshooting

#### Common Issues

1. **Terraform Error: "Subscription not found"**
   ```bash
   # Verify subscription
   az account show
   az account set --subscription "YOUR_SUBSCRIPTION_ID"
   ```

2. **Container App Not Starting**
   - Check logs: Azure Portal → Container App → Log stream
   - Verify SMTP credentials are correct
   - Ensure Docker image exists on Docker Hub

3. **SMTP Authentication Failed**
   - Use Gmail App Password, not regular password
   - Enable 2FA on your Google account first
   - Verify credentials in `terraform.tfvars`

4. **Terraform State Lock**
   ```bash
   # If state is locked (force unlock - use with caution)
   terraform force-unlock LOCK_ID
   ```

5. **High Costs**
   - Verify scale-to-zero is working: Check metrics in Azure Portal
   - Review Log Analytics data ingestion
   - Consider setting retention to 7 days for dev environments

## 🎯 Future Roadmap

- [x] AI-powered insights with ML.NET
- [x] Email notifications (registration confirmation, password reset)
- [ ] Mobile app integration
- [ ] Social features and habit sharing
- [ ] Advanced analytics and reporting
- [ ] API endpoints for third-party integration
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
