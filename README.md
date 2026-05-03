# 🤖 AiNewsFeed

A smart, local RSS aggregator for AI news built with **ASP.NET Core 8**, **Entity Framework Core**, and **Bootstrap 5**. Automatically extracts model names (Kimi K2.6, Claude Opus, Codex 5.5, GPT-5, etc.) and companies from article text for intelligent filtering.

> **Status:** ✅ Functional — fetches & displays articles from **55+ curated sources** including RSS, Reddit, Hacker News, ArXiv, and Google News RSS.  
> **Smart Tags:** Auto-detects AI models and companies mentioned in each article.

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
- [Source Types](#source-types)
- [Troubleshooting](#troubleshooting)
- [Contributing](#contributing)
- [License](#license)

---

## ✨ Features

- 📰 Aggregates AI news from **55+ curated sources**
  - **Official blogs:** Moonshot AI, Anthropic, OpenAI, DeepMind, Meta AI, Mistral, Cohere, NVIDIA, Hugging Face
  - **Tech news:** MIT Technology Review, Wired, The Verge, TechCrunch, VentureBeat, Ars Technica
  - **Research:** ArXiv (CS.AI, CS.LG, CS.CL, CS.CV), BAIR, Microsoft Research
  - **Community:** Reddit (r/LocalLLaMA, r/MachineLearning), Hacker News Algolia
  - **News aggregators:** Google News RSS for specific model queries
- 🏷️ **Auto-extracts** model names and companies from article text
- 🔍 **Smart filters:** Filter by Source, Model, Company, Unread status, or full-text search
- 🎨 Clean **Bootstrap 5** dark-themed frontend with real-time refresh
- ⚡ Async feed fetching with **Polly** resilience policies (3 retries with exponential backoff)
- 🗄️ **EF Core + SQL Server** with automatic database seeding
- 🔄 One-click refresh with loading spinner
- 🧹 Automatic soft-delete of articles older than retention period
- 🔗 URL normalization (strip UTM params, force HTTPS, deduplication)
- 📊 Source health tracking (auto-disable feeds after consecutive failures)

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
```

### 2. Configure your database connection

Open `appsettings.json` and update the connection string:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=YOUR_SERVER\SQLEXPRESS;Database=AiNewsFeedDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
}
```

Replace `YOUR_SERVER\SQLEXPRESS` with your actual SQL Server instance name.

> 💡 **Tip:** If using SQL Server Express locally, it's usually `localhost\SQLEXPRESS`

### 3. Restore packages & build

```bash
dotnet restore
dotnet build
```

### 4. Run database migrations (EF Core will auto-create & seed)

```bash
dotnet ef database update
```

> If `dotnet ef` is not found: `dotnet tool install --global dotnet-ef`

### 5. Run the application

```bash
dotnet run
```

### 6. Open in browser

Navigate to: `https://localhost:5001` or `http://localhost:5000`

> The frontend is served as a static SPA from `wwwroot/`. API endpoints are available at `/api/*`.

---

## 🗄️ Database Setup

The application uses EF Core Code-First with automatic seeding:

| Entity     | Description                                              |
| ---------- | -------------------------------------------------------- |
| FeedSource | 55+ pre-seeded AI news sources (RSS, Reddit, HN, etc.)   |
| Article    | Fetched articles with metadata, model tags, company tags |

On first run, EF Core will:

1. Create the `AiNewsFeedDb` database
2. Apply migrations
3. Seed all 55+ feed sources with proper categorization and keyword filters

---

## ⚙️ Configuration

### appsettings.json

| Setting                               | Description                  |
| ------------------------------------- | ---------------------------- |
| `ConnectionStrings:DefaultConnection` | SQL Server connection string |
| `Logging:LogLevel`                    | Console/file logging levels  |

### Feed Settings (optional)

Add to `appsettings.json` to customize fetch behavior:

```json
"FeedSettings": {
  "DelayBetweenRequestsMs": 500,
  "MaxConsecutiveFailures": 5,
  "ArticleRetentionDays": 90
}
```

### Environment-Specific Settings

Create `appsettings.Development.json` (gitignored) for local overrides:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=..\SQLEXPRESS;Database=AiNewsFeedDb;Trusted_Connection=True;TrustServerCertificate=True;MultipleActiveResultSets=true"
  }
}
```

---

## 📁 Project Structure

```text
AiNewsFeed/
├── Controllers/          # API controllers (Articles, Feeds, Sources)
│   ├── ArticlesController.cs
│   ├── FeedsController.cs
│   └── SourcesController.cs
├── Models/               # Domain entities
│   ├── Article.cs
│   ├── FeedSource.cs
│   └── Enums.cs          # FeedSourceType, SourcePriority
├── DTOs/                 # Data transfer objects
│   ├── ArticleResponseDto.cs
│   ├── FeedSourceResponseDto.cs
│   ├── FeedRefreshResponseDto.cs
│   └── UpdateReadStatusRequestDto.cs
├── Data/                 # DbContext & migrations
│   └── AppDbContext.cs
├── Services/             # Business logic
│   ├── IFeedFetcherService.cs
│   └── FeedFetcherService.cs
├── wwwroot/              # Static frontend files
│   ├── index.html        # Main SPA shell
│   ├── css/
│   │   └── style.css     # Custom dark theme styles
│   └── js/
│       └── app.js        # Frontend logic
├── appsettings.json      # Configuration template
├── Program.cs            # App startup & DI
└── AiNewsFeed.csproj     # Project file
```

---

## 🔌 API Endpoints

### Articles

| Method | Endpoint                     | Description                          |
| ------ | ---------------------------- | ------------------------------------ |
| GET    | `/api/articles`              | List articles (paginated, filterable) |
| GET    | `/api/articles/models`       | Get all detected models with counts  |
| GET    | `/api/articles/companies`    | Get all detected companies with counts |
| PATCH  | `/api/articles/{id}/read`    | Mark single article read/unread      |
| PATCH  | `/api/articles/read-all`     | Mark all articles as read            |

#### Query Parameters for `/api/articles`

| Parameter | Type   | Description                          |
| --------- | ------ | ------------------------------------ |
| `source`  | string | Filter by feed source name           |
| `model`   | string | Filter by mentioned model (e.g. `kimi-k2.6`) |
| `company` | string | Filter by mentioned company (e.g. `moonshot`) |
| `unread`  | bool   | Show only unread articles            |
| `search`  | string | Full-text search in title/summary    |
| `page`    | int    | Page number (default: 1)             |
| `pageSize`| int    | Items per page (default: 50, max: 100) |

### Feeds

| Method | Endpoint             | Description              |
| ------ | -------------------- | ------------------------ |
| POST   | `/api/feeds/refresh` | Trigger manual refresh   |

### Sources

| Method | Endpoint       | Description              |
| ------ | -------------- | ------------------------ |
| GET    | `/api/sources` | List all feed sources    |

---

## 📡 Source Types

The fetcher supports multiple source types, each handled differently:

| Type        | Examples                              | Notes                                      |
| ----------- | ------------------------------------- | ------------------------------------------ |
| `Rss`       | Tech blogs, research labs, arXiv      | Standard RSS/Atom parsing                  |
| `GoogleNews`| Google News RSS search feeds          | Keyword-targeted news aggregation          |
| `RedditJson`| r/LocalLLaMA, r/MachineLearning       | Reddit JSON API (no auth required)         |
| `HackerNews`| HN Algolia search                     | Community discussions about AI models      |

Each source can have:
- **Priority:** Critical → High → Medium → Low (affects fetch order)
- **RequiredKeywords:** Comma-separated filter — only articles matching these keywords are ingested
- **AlwaysInclude:** Bypass keyword filter (useful for official model announcement blogs)

---

## 🐛 Troubleshooting

### ❌ "Login failed for user" or connection errors

- Verify SQL Server is running
- Check Windows Authentication is enabled
- Ensure `TrustServerCertificate=True` is in the connection string
- Try `Server=localhost\SQLEXPRESS` or `Server=.` or `Server=(localdb)\MSSQLLocalDB`

### ❌ "dotnet ef" command not found

```bash
dotnet tool install --global dotnet-ef
```

### ❌ Frontend not loading articles

- Check browser console for CORS errors
- Verify API is running on expected port
- Check `appsettings.json` has valid connection string
- Ensure `wwwroot/index.html`, `css/style.css`, and `js/app.js` exist

### ❌ Model/Company dropdowns are empty

- Ensure you ran `dotnet ef database update` after adding new fields
- Click **Refresh** button to fetch new articles (old articles won't have tags)
- Check `/api/articles/models` and `/api/articles/companies` in browser — they should return JSON objects

### ❌ Feed sources failing repeatedly

- Some RSS feeds change URLs or block automated requests
- Failed sources are automatically disabled after 5 consecutive failures
- Check logs or `/api/sources` to see `lastErrorMessage` for each source

---

## 🤝 Contributing

This project is open for contributions! Areas to help:

- [ ] Add more RSS sources or custom feed URL support
- [ ] Add PostgreSQL / SQLite support
- [ ] Add unit tests for FeedFetcherService
- [ ] Docker support
- [ ] Add email/Slack/Discord notifications for new articles matching specific models
- [ ] Add article bookmarking / favorites
- [ ] Add sentiment analysis or summarization

1. Fork the repo
2. Create a feature branch: `git checkout -b feature/my-feature`
3. Commit your changes
4. Push and open a Pull Request

---

## 📄 License

This project is licensed under the [MIT License](LICENSE).

---

## 🙏 Acknowledgments

- [CodeHollow.FeedReader](https://github.com/codehollow/FeedReader) — RSS/Atom parsing
- [Polly](https://github.com/App-vNext/Polly) — Resilience policies
- [Bootstrap 5](https://getbootstrap.com/) — UI framework
- [Bootstrap Icons](https://icons.getbootstrap.com/) — Icon set

---

_Built with ❤️ at Masco Group Bangladesh_
