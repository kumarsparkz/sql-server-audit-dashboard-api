# SQL Server Audit Dashboard

A comprehensive real-time monitoring and auditing dashboard for SQL Server environments built with .NET 8 Web API backend and React frontend.

![Dashboard Status](https://img.shields.io/badge/Status-Active-brightgreen) ![React](https://img.shields.io/badge/React-18.2.0-blue) ![TypeScript](https://img.shields.io/badge/TypeScript-4.9.5-blue) ![Material--UI](https://img.shields.io/badge/Material--UI-5.x-blue) ![.NET](https://img.shields.io/badge/.NET-Core-purple)

[![GitHub stars](https://img.shields.io/github/stars/kumarsparkz/sql-server-audit-dashboard-web)](https://github.com/kumarsparkz/sql-server-audit-dashboard-web/stargazers)
[![GitHub forks](https://img.shields.io/github/forks/kumarsparkz/sql-server-audit-dashboard-web)](https://github.com/kumarsparkz/sql-server-audit-dashboard-web/network)
[![GitHub issues](https://img.shields.io/github/issues/kumarsparkz/sql-server-audit-dashboard-web)](https://github.com/kumarsparkz/sql-server-audit-dashboard-web/issues)
[![GitHub license](https://img.shields.io/github/license/kumarsparkz/sql-server-audit-dashboard-web)](https://github.com/kumarsparkz/sql-server-audit-dashboard-web/blob/main/LICENSE)

## 🔗 Related Repositories

This is the **backend api application**. For the complete solution, you'll also need:

- 🔧 **[Frontend web](https://github.com/kumarsparkz/sql-server-audit-dashboard-web)** - A modern, real-time SQL Server monitoring and audit dashboard built with React
- 📚 **[Documentation](https://github.com/kumarsparkz/sql-server-audit-dashboard-web/blob/main/README.md)** - API documentation and deployment guides

### 🏗️ **Architecture Overview**
```
┌─────────────────────┐    HTTP/HTTPS     ┌──────────────────────┐
│   React Frontend    │ ◄─────────────── │    .NET Core API     │
│  (This Repository)  │                   │   (Backend Repo)     │
└─────────────────────┘                   └──────────────────────┘
         │                                           │
         │ Real-time Updates                         │
         └────────── SignalR ─────────────────────────┘
```

## 🚀 Features

- **Real-time Server Monitoring** - Live performance metrics (CPU, Memory, Disk usage)
- **Alert Management** - Configurable alerts with severity levels (Critical, Warning, Info)
- **Security Auditing** - Track security events and failed login attempts
- **Multi-Server Support** - Monitor multiple SQL Server instances across environments
- **Dashboard Analytics** - Visual charts and trends for performance analysis
- **User Authentication** - Secure access with role-based permissions
- **Real-time Notifications** - SignalR integration for live updates

## 🏗️ Architecture

### Backend (.NET 8 Web API)
- **Entity Framework Core** - Database operations and migrations
- **SignalR** - Real-time communication
- **Authentication** - JWT-based security
- **Background Services** - Automated metric collection
- **RESTful APIs** - Comprehensive endpoint coverage

### Frontend (React)
- **Material-UI** - Modern responsive design ([Repository](https://github.com/kumarsparkz/sql-server-audit-dashboard-web))
- **Real-time Updates** - SignalR client integration
- **Interactive Charts** - Performance visualization
- **Responsive Layout** - Mobile-friendly interface

## 📋 Prerequisites

- **.NET 8 SDK** or later
- **Node.js 18+** and npm
- **SQL Server** (LocalDB, Express, or Full)
- **Visual Studio Code** or Visual Studio 2022

## 🛠️ Installation & Setup

### 1. Clone the Repository
```bash
git clone <repository-url>
cd SQL_Server_Audit_Dashboard
```

### 2. Backend Setup (API)
```bash
cd Audit_api/Audit_api

# Restore NuGet packages
dotnet restore

# Update database connection string in appsettings.json
# Default: Server=(localdb)\mssqllocaldb;Database=AuditDashboard;Trusted_Connection=true;

# Apply database migrations
dotnet ef database update

# Run the API
dotnet run
```

The API will be available at: `https://localhost:7001`

### 3. Frontend Setup (React)
```bash
cd audit-dashboard-frontend

# Install dependencies
npm install

# Start the development server
npm start
```

The frontend will be available at: `http://localhost:3000`

## 🔧 Configuration

### Database Connection
Update `appsettings.json` with your SQL Server connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=your-server;Database=AuditDashboard;Trusted_Connection=true;"
  }
}
```

### CORS Settings
The API is configured to allow requests from `http://localhost:3000` by default. Update in `Program.cs` if needed:

```csharp
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReactApp", policy =>
    {
        policy.WithOrigins("http://localhost:3000", "https://localhost:3000")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});
```

## 📊 API Endpoints

### Dashboard
- `GET /api/dashboard/overview` - Main dashboard data
- `GET /api/dashboard/servers` - Server list with metrics
- `GET /api/dashboard/servers/{id}` - Specific server details

### Authentication
- `POST /api/auth/login` - User login
- `POST /api/auth/register` - User registration

### Alerts
- `GET /api/alerts` - All alerts
- `GET /api/alerts/recent` - Recent alerts
- `POST /api/alerts/acknowledge/{id}` - Acknowledge alert

### Servers
- `GET /api/servers` - Monitored servers
- `POST /api/servers` - Add new server
- `PUT /api/servers/{id}` - Update server
- `DELETE /api/servers/{id}` - Remove server

### Security
- `GET /api/security/events` - Security audit events
- `GET /api/security/failed-logins` - Failed login attempts

## 🗄️ Database Schema

### Key Tables
- **MonitoredServers** - SQL Server instances being monitored
- **ServerMetrics** - Performance metrics (CPU, Memory, Disk)
- **ActiveAlerts** - Current alerts and notifications
- **SecurityEvents** - Audit trail of security events
- **DashboardUsers** - Application users and roles

## 🔐 Default Credentials

**Admin User:**
- Username: `admin`
- Password: ``

*Change these credentials immediately in production!*

## 🚦 Running in Production

### Backend Deployment
```bash
# Publish the API
dotnet publish -c Release -o ./publish

# Run with production settings
ASPNETCORE_ENVIRONMENT=Production dotnet Audit_api.dll
```

### Frontend Deployment
```bash
# Build for production
npm run build

# Serve the built files using a web server
# (nginx, IIS, or any static file server)
```

## 📈 Monitoring Setup

### Adding SQL Servers
1. Navigate to the Servers page
2. Click "Add Server"
3. Provide server details and connection string
4. Configure monitoring intervals and alert thresholds

### Setting Up Alerts
1. Go to Alerts configuration
2. Define metric thresholds (CPU > 80%, Memory > 90%, etc.)
3. Set notification preferences
4. Configure alert escalation rules

## 🛠️ Development

### Running Tests
```bash
# Backend tests
cd Audit_api/Audit_api
dotnet test

# Frontend tests
cd audit-dashboard-frontend
npm test
```

### Database Migrations
```bash
# Add new migration
dotnet ef migrations add MigrationName

# Apply migrations
dotnet ef database update

# Remove last migration
dotnet ef migrations remove
```

### API Documentation
Swagger UI is available at: `https://localhost:7001/swagger`

## 🔧 Troubleshooting

### Common Issues

**CORS Errors:**
- Ensure the React app is running on `http://localhost:3000`
- Check CORS policy in `Program.cs`

**Database Connection:**
- Verify SQL Server is running
- Check connection string in `appsettings.json`
- Ensure database exists and migrations are applied

**Build Errors:**
- Run `dotnet clean` and `dotnet restore`
- Check for missing NuGet packages
- Verify .NET 8 SDK is installed

## 📝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/AmazingFeature`)
3. Commit your changes (`git commit -m 'Add some AmazingFeature'`)
4. Push to the branch (`git push origin feature/AmazingFeature`)
5. Open a Pull Request

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 🤝 Support

For support and questions:
- Create an issue in the GitHub repository
- Check the troubleshooting section above
- Review the API documentation at `/swagger`

## 🚀 Roadmap

- [ ] Email/SMS alert notifications
- [ ] Custom dashboard widgets
- [ ] Advanced analytics and reporting
- [ ] Multi-tenant support
- [ ] Mobile app companion
- [ ] Integration with popular monitoring tools

---

<div align="center">

**Built with ❤️ using .NET 8 and React**

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=flat&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)


![GitHub stars](https://img.shields.io/github/stars/kumarsparkz/sql-server-audit-dashboard-api)
![GitHub forks](https://img.shields.io/github/forks/kumarsparkz/sql-server-audit-dashboard-api)
![GitHub issues](https://img.shields.io/github/issues/kumarsparkz/sql-server-audit-dashboard-api)
![GitHub license](https://img.shields.io/github/license/kumarsparkz/sql-server-audit-dashboard-api)

*Star ⭐ this repository if you find it helpful!*

</div>


