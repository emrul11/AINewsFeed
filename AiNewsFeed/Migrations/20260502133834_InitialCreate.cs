using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AiNewsFeed.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Articles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Summary = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Url = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Source = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Author = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    PublishedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FetchedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Articles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FeedSources",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FeedUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastFetchedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    LastFetchStatus = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    LastErrorMessage = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ConsecutiveFailures = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FeedSources", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "FeedSources",
                columns: new[] { "Id", "ConsecutiveFailures", "FeedUrl", "IsActive", "LastErrorMessage", "LastFetchStatus", "LastFetchedAt", "Name" },
                values: new object[,]
                {
                    { 1, 0, "https://www.technologyreview.com/feed/", true, null, null, null, "MIT Technology Review" },
                    { 2, 0, "https://www.wired.com/feed/tag/ai/latest/rss", true, null, null, null, "Wired - AI" },
                    { 3, 0, "https://www.theverge.com/rss/ai-artificial-intelligence/index.xml", true, null, null, null, "The Verge - AI" },
                    { 4, 0, "https://venturebeat.com/category/ai/feed/", true, null, null, null, "VentureBeat - AI" },
                    { 5, 0, "https://techcrunch.com/category/artificial-intelligence/feed/", true, null, null, null, "TechCrunch - AI" },
                    { 6, 0, "https://arstechnica.com/tag/artificial-intelligence/feed/", true, null, null, null, "Ars Technica - AI" },
                    { 7, 0, "https://spectrum.ieee.org/rss/topic/artificial-intelligence", true, null, null, null, "IEEE Spectrum - AI" },
                    { 8, 0, "https://analyticsindiamag.com/feed/", true, null, null, null, "Analytics India Magazine" },
                    { 9, 0, "https://www.artificialintelligence-news.com/feed/", true, null, null, null, "AI News" },
                    { 10, 0, "https://www.marktechpost.com/feed/", true, null, null, null, "MarkTechPost" },
                    { 11, 0, "https://www.unite.ai/feed/", true, null, null, null, "Unite.AI" },
                    { 12, 0, "https://machinelearningmastery.com/feed/", true, null, null, null, "Machine Learning Mastery" },
                    { 13, 0, "https://towardsdatascience.com/feed", true, null, null, null, "Towards Data Science" },
                    { 14, 0, "https://ai.googleblog.com/feeds/posts/default", true, null, null, null, "Google AI Blog" },
                    { 15, 0, "https://openai.com/blog/rss.xml", true, null, null, null, "OpenAI Blog" },
                    { 16, 0, "https://deepmind.com/blog/feed/basic/", true, null, null, null, "DeepMind Blog" },
                    { 17, 0, "https://www.microsoft.com/en-us/research/blog/feed/", true, null, null, null, "Microsoft Research" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Articles_IsDeleted",
                table: "Articles",
                column: "IsDeleted");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_IsRead",
                table: "Articles",
                column: "IsRead");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_PublishedAt",
                table: "Articles",
                column: "PublishedAt",
                descending: new bool[0]);

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Source",
                table: "Articles",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_Url",
                table: "Articles",
                column: "Url",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FeedSources_FeedUrl",
                table: "FeedSources",
                column: "FeedUrl");

            migrationBuilder.CreateIndex(
                name: "IX_FeedSources_IsActive",
                table: "FeedSources",
                column: "IsActive");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Articles");

            migrationBuilder.DropTable(
                name: "FeedSources");
        }
    }
}
