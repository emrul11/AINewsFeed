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
        });

        // FeedSource configurations
        modelBuilder.Entity<FeedSource>(entity =>
        {
            entity.HasIndex(f => f.FeedUrl);
            entity.HasIndex(f => f.IsActive);
        });

        // Seed 17 AI news feed sources
        modelBuilder.Entity<FeedSource>().HasData(
            new FeedSource { Id = 1, Name = "MIT Technology Review", FeedUrl = "https://www.technologyreview.com/feed/", IsActive = true },
            new FeedSource { Id = 2, Name = "Wired - AI", FeedUrl = "https://www.wired.com/feed/tag/ai/latest/rss", IsActive = true },
            new FeedSource { Id = 3, Name = "The Verge - AI", FeedUrl = "https://www.theverge.com/rss/ai-artificial-intelligence/index.xml", IsActive = true },
            new FeedSource { Id = 4, Name = "VentureBeat - AI", FeedUrl = "https://venturebeat.com/category/ai/feed/", IsActive = true },
            new FeedSource { Id = 5, Name = "TechCrunch - AI", FeedUrl = "https://techcrunch.com/category/artificial-intelligence/feed/", IsActive = true },
            new FeedSource { Id = 6, Name = "Ars Technica - AI", FeedUrl = "https://arstechnica.com/tag/artificial-intelligence/feed/", IsActive = true },
            new FeedSource { Id = 7, Name = "IEEE Spectrum - AI", FeedUrl = "https://spectrum.ieee.org/rss/topic/artificial-intelligence", IsActive = true },
            new FeedSource { Id = 8, Name = "Analytics India Magazine", FeedUrl = "https://analyticsindiamag.com/feed/", IsActive = true },
            new FeedSource { Id = 9, Name = "AI News", FeedUrl = "https://www.artificialintelligence-news.com/feed/", IsActive = true },
            new FeedSource { Id = 10, Name = "MarkTechPost", FeedUrl = "https://www.marktechpost.com/feed/", IsActive = true },
            new FeedSource { Id = 11, Name = "Unite.AI", FeedUrl = "https://www.unite.ai/feed/", IsActive = true },
            new FeedSource { Id = 12, Name = "Machine Learning Mastery", FeedUrl = "https://machinelearningmastery.com/feed/", IsActive = true },
            new FeedSource { Id = 13, Name = "Towards Data Science", FeedUrl = "https://towardsdatascience.com/feed", IsActive = true },
            new FeedSource { Id = 14, Name = "Google AI Blog", FeedUrl = "https://ai.googleblog.com/feeds/posts/default", IsActive = true },
            new FeedSource { Id = 15, Name = "OpenAI Blog", FeedUrl = "https://openai.com/blog/rss.xml", IsActive = true },
            new FeedSource { Id = 16, Name = "DeepMind Blog", FeedUrl = "https://deepmind.com/blog/feed/basic/", IsActive = true },
            new FeedSource { Id = 17, Name = "Microsoft Research", FeedUrl = "https://www.microsoft.com/en-us/research/blog/feed/", IsActive = true }
        );
    }
}