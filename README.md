# 🤖 AiNewsFeed

A lightweight, local RSS aggregator for AI news built with **ASP.NET Core 8**, **Entity Framework Core**, and **Bootstrap 5**.

> **Status:** ✅ Functional — fetches & displays articles from 17 seeded AI news sources.  
> **Pending:** Spinner bugfix on refresh button (JavaScript `isRefreshing` flag).

---

## 📋 Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Database Setup](#database-setup)
- [Configuration](#configuration)
- [Project Structure](#project-structure)
- [API Endpoints](#api-endpoints)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## ✨ Features

- 📰 Aggregates AI news from **17 curated RSS sources**
- ⚡ Async feed fetching with **Polly** resilience policies
- 🗄️ **EF Core + SQL Server** with automatic database seeding
- 🎨 Clean **Bootstrap 5** frontend with real-time refresh
- 🔍 Search and filter articles
- 🔄 One-click refresh with loading spinner

---

## 🛠️ Tech Stack

| Layer       | Technology                                  |
| ----------- | ------------------------------------------- |
| Backend     | ASP.NET Core 8 Web API                      |
| ORM         | Entity Framework Core 8                     |
| Database    | SQL Server (LocalDB / Express / Full)       |
| RSS Parsing | CodeHollow.FeedReader                       |
| Resilience  | Polly                                       |
| Frontend    | Bootstrap 5 + Vanilla JavaScript            |
| Auth        | Windows Authentication (Trusted Connection) |

---

## 📦 Prerequisites

Before you begin, ensure you have:

| Requirement | Version          | Download                                                                    |
| ----------- | ---------------- | --------------------------------------------------------------------------- |
| .NET SDK    | 8.0+             | [Download](https://dotnet.microsoft.com/download/dotnet/8.0)                |
| SQL Server  | 2019+ or Express | [Download](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) |
| Git         | Any              | [Download](https://git-scm.com/downloads)                                   |

> **Windows Auth Note:** This project uses `Trusted_Connection=True`. You must run SQL Server with Windows Authentication enabled (default for Express).

---

## 🚀 Getting Started

### 1. Clone the repository

```bash
git clone https://github.com/emrul11/AINewsFeed.git
cd AiNewsFeed
2. Configure your database connection
Open appsettings.json and update the connection string:
JSON
Copy
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER\\SQLEXPRESS;Database=AiNewsFeedDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
Replace YOUR_SERVER\\SQLEXPRESS with your actual SQL Server instance name.
💡 Tip: If using SQL Server Express locally, it's usually localhost\\SQLEXPRESS or EMRUL\\SQLEXPRESS.
3. Restore packages & build
bash
Copy
dotnet restore
dotnet build
4. Run database migrations (EF Core will auto-create & seed)
bash
Copy
dotnet ef database update
If dotnet ef is not found: dotnet tool install --global dotnet-ef
5. Run the application
bash
Copy
dotnet run
6. Open in browser
Navigate to: https://localhost:5001 or http://localhost:5000
🗄️ Database Setup
The application uses EF Core Code-First with automatic seeding:
Table
Entity	Description
FeedSource	17 pre-seeded AI news RSS sources
Article	Fetched articles with metadata
On first run, EF Core will:
Create the AiNewsFeedDb database
Apply migrations
Seed all 17 feed sources
⚙️ Configuration
appsettings.json
Table
Setting	Description
ConnectionStrings:DefaultConnection	SQL Server connection string
Logging:LogLevel	Console/file logging levels
Environment-Specific Settings
Create appsettings.Development.json (gitignored) for local overrides:
JSON
Copy
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=EMRUL\\SQLEXPRESS;Database=AiNewsFeedDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
📁 Project Structure
plain
Copy
AiNewsFeed/
├── Controllers/          # API controllers (Articles, Feeds)
├── Models/               # Domain entities (FeedSource, Article)
├── DTOs/                 # Data transfer objects
├── Data/                 # DbContext & migrations
├── Services/             # Business logic (FeedFetchService, etc.)
├── wwwroot/              # Static frontend files
│   ├── index.html        # Main UI
│   ├── css/              # Bootstrap + custom styles
│   └── js/               # Frontend logic
├── appsettings.json      # Configuration template
├── Program.cs            # App startup & DI
└── AiNewsFeed.csproj     # Project file
🔌 API Endpoints
Table
Method	Endpoint	Description
GET	/api/articles	List all articles
GET	/api/articles/{id}	Get article by ID
GET	/api/feeds	List all feed sources
POST	/api/feeds/refresh	Trigger manual refresh
GET	/api/feeds/status	Get fetch status
🐛 Troubleshooting
❌ "Login failed for user" or connection errors
Verify SQL Server is running
Check Windows Authentication is enabled
Ensure TrustServerCertificate=True is in the connection string
Try Server=localhost\\SQLEXPRESS or Server=. or Server=(localdb)\\MSSQLLocalDB
❌ "dotnet ef" command not found
bash
Copy
dotnet tool install --global dotnet-ef
❌ Frontend not loading articles
Check browser console for CORS errors
Verify API is running on expected port
Check appsettings.json has valid connection string
❌ Spinner doesn't stop on refresh
Fix: In wwwroot/js/app.js, ensure isRefreshing flag is reset in both .then() and .catch() of the fetch promise.
See inline comment in the file for the exact patch.
🤝 Contributing
This project is open for contributions! Areas to help:
[ ] Fix refresh spinner bug
[ ] Add unit tests
[ ] Support for custom RSS feed URLs
[ ] Docker support
[ ] Dark mode toggle
Fork the repo
Create a feature branch: git checkout -b feature/my-feature
Commit your changes
Push and open a Pull Request
📄 License
This project is licensed under the MIT License.
🙏 Acknowledgments
CodeHollow.FeedReader — RSS/Atom parsing
Polly — Resilience policies
Bootstrap 5 — UI framework
Built with ❤️ at Masco Group Bangladesh
```
