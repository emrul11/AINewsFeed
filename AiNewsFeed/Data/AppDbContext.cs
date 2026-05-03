using Microsoft.EntityFrameworkCore;
using AiNewsFeed.Models;

namespace AiNewsFeed.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Article> Articles => Set<Article>();
    public DbSet<FeedSource> FeedSources => Set<FeedSource>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Article configurations
        modelBuilder.Entity<Article>(entity =>
        {
            entity.HasIndex(a => a.Url).IsUnique();
            entity.HasIndex(a => a.PublishedAt).IsDescending();
            entity.HasIndex(a => a.Source);
            entity.HasIndex(a => a.IsRead);
            entity.HasIndex(a => a.IsDeleted);
            entity.HasIndex(a => a.MentionedModels); // NEW
            entity.HasIndex(a => a.ContentHash);     // NEW
            entity.HasIndex(a => a.FetchedAt);       // NEW
        });

        // FeedSource configurations
        modelBuilder.Entity<FeedSource>(entity =>
        {
            entity.HasIndex(f => f.FeedUrl);
            entity.HasIndex(f => f.IsActive);
            entity.HasIndex(f => f.Type);
            entity.HasIndex(f => f.Priority);
        });

        // Seed sources — 50+ categorized feeds
        // OFFICIAL / CRITICAL PRIORITY (Model announcements, research labs)
        modelBuilder.Entity<FeedSource>().HasData(
            new FeedSource { Id = 1, Name = "Moonshot AI (Kimi)", FeedUrl = "https://www.moonshot.cn/news/rss", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 2, Name = "Anthropic Blog", FeedUrl = "https://www.anthropic.com/blog/rss.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 3, Name = "OpenAI Blog", FeedUrl = "https://openai.com/blog/rss.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 4, Name = "Google DeepMind Blog", FeedUrl = "https://deepmind.google/discover/blog/rss.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 5, Name = "Google AI Blog", FeedUrl = "https://blog.google/technology/ai/rss/", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 6, Name = "Meta AI Blog", FeedUrl = "https://ai.meta.com/blog/rss/", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 7, Name = "Mistral AI Blog", FeedUrl = "https://mistral.ai/news/rss.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 8, Name = "Cohere Blog", FeedUrl = "https://cohere.com/blog/rss", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 9, Name = "Stability AI Blog", FeedUrl = "https://stability.ai/blog/rss.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 10, Name = "NVIDIA AI Blog", FeedUrl = "https://blogs.nvidia.com/blog/category/artificial-intelligence/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },
            new FeedSource { Id = 11, Name = "Hugging Face Blog", FeedUrl = "https://huggingface.co/blog/feed.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Critical, AlwaysInclude = true, IsActive = true },

            // HIGH PRIORITY (Research institutions, major dev platforms)
            new FeedSource { Id = 12, Name = "BAIR Blog (Berkeley)", FeedUrl = "https://bair.berkeley.edu/blog/feed.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 13, Name = "Microsoft Research", FeedUrl = "https://www.microsoft.com/en-us/research/blog/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 14, Name = "PyTorch Blog", FeedUrl = "https://pytorch.org/blog/atom.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 15, Name = "TensorFlow Blog", FeedUrl = "https://blog.tensorflow.org/feeds/posts/default", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 16, Name = "LangChain Blog", FeedUrl = "https://blog.langchain.dev/rss/", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },

            // MEDIUM PRIORITY (Tech news — filtered by keywords)
            new FeedSource { Id = 17, Name = "MIT Technology Review", FeedUrl = "https://www.technologyreview.com/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,language model,Claude,GPT,Kimi,Gemini,Codex,OpenAI,Anthropic,Moonshot,DeepMind", IsActive = true },
            new FeedSource { Id = 18, Name = "Wired - AI", FeedUrl = "https://www.wired.com/feed/tag/ai/latest/rss", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", IsActive = true },
            new FeedSource { Id = 19, Name = "The Verge - AI", FeedUrl = "https://www.theverge.com/rss/ai-artificial-intelligence/index.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", IsActive = true },
            new FeedSource { Id = 20, Name = "VentureBeat - AI", FeedUrl = "https://venturebeat.com/category/ai/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", IsActive = true },
            new FeedSource { Id = 21, Name = "TechCrunch - AI", FeedUrl = "https://techcrunch.com/category/artificial-intelligence/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", IsActive = true },
            new FeedSource { Id = 22, Name = "Ars Technica - AI", FeedUrl = "https://arstechnica.com/tag/artificial-intelligence/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", IsActive = true },
            new FeedSource { Id = 23, Name = "IEEE Spectrum - AI", FeedUrl = "https://spectrum.ieee.org/rss/topic/artificial-intelligence", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, RequiredKeywords = "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", IsActive = true },

            // AI-SPECIFIC PUBLICATIONS
            new FeedSource { Id = 24, Name = "AI News", FeedUrl = "https://www.artificialintelligence-news.com/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },
            new FeedSource { Id = 25, Name = "MarkTechPost", FeedUrl = "https://www.marktechpost.com/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },
            new FeedSource { Id = 26, Name = "Unite.AI", FeedUrl = "https://www.unite.ai/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },
            new FeedSource { Id = 27, Name = "Machine Learning Mastery", FeedUrl = "https://machinelearningmastery.com/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },
            new FeedSource { Id = 28, Name = "Synced Review", FeedUrl = "https://syncedreview.com/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },
            new FeedSource { Id = 29, Name = "Analytics India Magazine", FeedUrl = "https://analyticsindiamag.com/feed/", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },

            // NEWSLETTERS / SUBSTACK (Often break news first)
            new FeedSource { Id = 30, Name = "Import AI (Jack Clark)", FeedUrl = "https://importai.substack.com/feed", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 31, Name = "The Batch (DeepLearning.AI)", FeedUrl = "https://www.deeplearning.ai/the-batch/rss/", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 32, Name = "One Useful Thing (Ethan Mollick)", FeedUrl = "https://www.oneusefulthing.org/rss", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 33, Name = "Last Week in AI", FeedUrl = "https://lastweekin.ai/feed", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 34, Name = "AI Weekly", FeedUrl = "https://aiweekly.co/rss", Type = FeedSourceType.Rss, Priority = SourcePriority.Medium, IsActive = true },

            // INDIVIDUAL EXPERTS
            new FeedSource { Id = 35, Name = "Lilian Weng", FeedUrl = "https://lilianweng.github.io/index.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 36, Name = "Sebastian Raschka", FeedUrl = "https://magazine.sebastianraschka.com/feed", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 37, Name = "Simon Willison", FeedUrl = "https://simonwillison.net/atom/everything/", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 38, Name = "Chip Huyen", FeedUrl = "https://huyenchip.com/feed.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 39, Name = "Eugene Yan", FeedUrl = "https://eugeneyan.com/writing/feed.xml", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },

            // ARXIV (Research papers — massive volume)
            new FeedSource { Id = 40, Name = "ArXiv CS.AI", FeedUrl = "https://rss.arxiv.org/rss/cs.AI", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 41, Name = "ArXiv CS.LG", FeedUrl = "https://rss.arxiv.org/rss/cs.LG", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 42, Name = "ArXiv CS.CL", FeedUrl = "https://rss.arxiv.org/rss/cs.CL", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 43, Name = "ArXiv CS.CV", FeedUrl = "https://rss.arxiv.org/rss/cs.CV", Type = FeedSourceType.Rss, Priority = SourcePriority.High, IsActive = true },

            // GOOGLE NEWS RSS (No API key needed — keyword search)
            new FeedSource { Id = 44, Name = "Google News - Kimi K2.6", FeedUrl = "https://news.google.com/rss/search?q=%22Kimi+K2.6%22+OR+%22Moonshot+AI%22&hl=en-US&gl=US&ceid=US:en", Type = FeedSourceType.GoogleNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 45, Name = "Google News - Claude Opus", FeedUrl = "https://news.google.com/rss/search?q=%22Claude+Opus%22+OR+%22Claude+4.6%22+OR+%22Claude+4.7%22&hl=en-US&gl=US&ceid=US:en", Type = FeedSourceType.GoogleNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 46, Name = "Google News - Codex", FeedUrl = "https://news.google.com/rss/search?q=%22Codex+5.5%22+OR+%22OpenAI+Codex%22&hl=en-US&gl=US&ceid=US:en", Type = FeedSourceType.GoogleNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 47, Name = "Google News - GPT-5", FeedUrl = "https://news.google.com/rss/search?q=%22GPT-5%22+OR+%22GPT-4.5%22+OR+%22OpenAI+o3%22&hl=en-US&gl=US&ceid=US:en", Type = FeedSourceType.GoogleNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 48, Name = "Google News - Gemini", FeedUrl = "https://news.google.com/rss/search?q=%22Gemini+2.5%22+OR+%22Google+Gemini%22+model&hl=en-US&gl=US&ceid=US:en", Type = FeedSourceType.GoogleNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 49, Name = "Google News - AI Models", FeedUrl = "https://news.google.com/rss/search?q=%22large+language+model%22+OR+%22LLM%22+release+announcement&hl=en-US&gl=US&ceid=US:en", Type = FeedSourceType.GoogleNews, Priority = SourcePriority.Medium, IsActive = true },

            // REDDIT JSON (Community buzz — often fastest signal)
            new FeedSource { Id = 50, Name = "Reddit r/LocalLLaMA", FeedUrl = "https://www.reddit.com/r/LocalLLaMA/.json?limit=25", Type = FeedSourceType.RedditJson, Priority = SourcePriority.Medium, RequiredKeywords = "Kimi,Claude,Codex,GPT,Gemini,Moonshot,Anthropic,OpenAI", IsActive = true },
            new FeedSource { Id = 51, Name = "Reddit r/MachineLearning", FeedUrl = "https://www.reddit.com/r/MachineLearning/.json?limit=25", Type = FeedSourceType.RedditJson, Priority = SourcePriority.Medium, RequiredKeywords = "Kimi,Claude,Codex,GPT,Gemini,Moonshot,Anthropic,OpenAI", IsActive = true },
            new FeedSource { Id = 52, Name = "Reddit r/singularity", FeedUrl = "https://www.reddit.com/r/singularity/.json?limit=25", Type = FeedSourceType.RedditJson, Priority = SourcePriority.Low, RequiredKeywords = "Kimi,Claude,Codex,GPT,Gemini,Moonshot", IsActive = true },

            // HACKER NEWS (Algolia search for specific models)
            new FeedSource { Id = 53, Name = "HN - Kimi/Moonshot", FeedUrl = "https://hn.algolia.com/api/v1/search?query=moonshot+ai+kimi&tags=story&hitsPerPage=30", Type = FeedSourceType.HackerNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 54, Name = "HN - Claude", FeedUrl = "https://hn.algolia.com/api/v1/search?query=claude+opus+anthropic&tags=story&hitsPerPage=30", Type = FeedSourceType.HackerNews, Priority = SourcePriority.High, IsActive = true },
            new FeedSource { Id = 55, Name = "HN - OpenAI/Codex", FeedUrl = "https://hn.algolia.com/api/v1/search?query=openai+codex+gpt&tags=story&hitsPerPage=30", Type = FeedSourceType.HackerNews, Priority = SourcePriority.High, IsActive = true }
        );
    }
}