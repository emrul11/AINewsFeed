using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AiNewsFeed.Migrations
{
    /// <inheritdoc />
    public partial class EnrichFeeds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeedSources",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "LastFetchStatus",
                table: "FeedSources",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastErrorMessage",
                table: "FeedSources",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FeedUrl",
                table: "FeedSources",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AddColumn<bool>(
                name: "AlwaysInclude",
                table: "FeedSources",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "FetchIntervalMinutes",
                table: "FeedSources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Priority",
                table: "FeedSources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RequiredKeywords",
                table: "FeedSources",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "FeedSources",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000);

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500);

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MentionedCompanies",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MentionedModels",
                table: "Articles",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "SearchVector",
                table: "Articles",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "SourceType",
                table: "Articles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://www.moonshot.cn/news/rss", 60, "Moonshot AI (Kimi)", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://www.anthropic.com/blog/rss.xml", 60, "Anthropic Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://openai.com/blog/rss.xml", 60, "OpenAI Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://deepmind.google/discover/blog/rss.xml", 60, "Google DeepMind Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://blog.google/technology/ai/rss/", 60, "Google AI Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://ai.meta.com/blog/rss/", 60, "Meta AI Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://mistral.ai/news/rss.xml", 60, "Mistral AI Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://cohere.com/blog/rss", 60, "Cohere Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://stability.ai/blog/rss.xml", 60, "Stability AI Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://blogs.nvidia.com/blog/category/artificial-intelligence/feed/", 60, "NVIDIA AI Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { true, "https://huggingface.co/blog/feed.xml", 60, "Hugging Face Blog", 4, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { false, "https://bair.berkeley.edu/blog/feed.xml", 60, "BAIR Blog (Berkeley)", 3, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { false, "https://www.microsoft.com/en-us/research/blog/feed/", 60, "Microsoft Research", 3, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { false, "https://pytorch.org/blog/atom.xml", 60, "PyTorch Blog", 3, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { false, "https://blog.tensorflow.org/feeds/posts/default", 60, "TensorFlow Blog", 3, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { false, "https://blog.langchain.dev/rss/", 60, "LangChain Blog", 3, null, 0 });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "AlwaysInclude", "FeedUrl", "FetchIntervalMinutes", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[] { false, "https://www.technologyreview.com/feed/", 60, "MIT Technology Review", 2, "AI,artificial intelligence,machine learning,LLM,language model,Claude,GPT,Kimi,Gemini,Codex,OpenAI,Anthropic,Moonshot,DeepMind", 0 });

            migrationBuilder.InsertData(
                table: "FeedSources",
                columns: new[] { "Id", "AlwaysInclude", "ConsecutiveFailures", "FeedUrl", "FetchIntervalMinutes", "IsActive", "LastErrorMessage", "LastFetchStatus", "LastFetchedAt", "Name", "Priority", "RequiredKeywords", "Type" },
                values: new object[,]
                {
                    { 18, false, 0, "https://www.wired.com/feed/tag/ai/latest/rss", 60, true, null, null, null, "Wired - AI", 2, "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", 0 },
                    { 19, false, 0, "https://www.theverge.com/rss/ai-artificial-intelligence/index.xml", 60, true, null, null, null, "The Verge - AI", 2, "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", 0 },
                    { 20, false, 0, "https://venturebeat.com/category/ai/feed/", 60, true, null, null, null, "VentureBeat - AI", 2, "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", 0 },
                    { 21, false, 0, "https://techcrunch.com/category/artificial-intelligence/feed/", 60, true, null, null, null, "TechCrunch - AI", 2, "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", 0 },
                    { 22, false, 0, "https://arstechnica.com/tag/artificial-intelligence/feed/", 60, true, null, null, null, "Ars Technica - AI", 2, "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", 0 },
                    { 23, false, 0, "https://spectrum.ieee.org/rss/topic/artificial-intelligence", 60, true, null, null, null, "IEEE Spectrum - AI", 2, "AI,artificial intelligence,machine learning,LLM,Claude,GPT,Kimi,Gemini,Codex", 0 },
                    { 24, false, 0, "https://www.artificialintelligence-news.com/feed/", 60, true, null, null, null, "AI News", 2, null, 0 },
                    { 25, false, 0, "https://www.marktechpost.com/feed/", 60, true, null, null, null, "MarkTechPost", 2, null, 0 },
                    { 26, false, 0, "https://www.unite.ai/feed/", 60, true, null, null, null, "Unite.AI", 2, null, 0 },
                    { 27, false, 0, "https://machinelearningmastery.com/feed/", 60, true, null, null, null, "Machine Learning Mastery", 2, null, 0 },
                    { 28, false, 0, "https://syncedreview.com/feed/", 60, true, null, null, null, "Synced Review", 2, null, 0 },
                    { 29, false, 0, "https://analyticsindiamag.com/feed/", 60, true, null, null, null, "Analytics India Magazine", 2, null, 0 },
                    { 30, false, 0, "https://importai.substack.com/feed", 60, true, null, null, null, "Import AI (Jack Clark)", 3, null, 0 },
                    { 31, false, 0, "https://www.deeplearning.ai/the-batch/rss/", 60, true, null, null, null, "The Batch (DeepLearning.AI)", 3, null, 0 },
                    { 32, false, 0, "https://www.oneusefulthing.org/rss", 60, true, null, null, null, "One Useful Thing (Ethan Mollick)", 3, null, 0 },
                    { 33, false, 0, "https://lastweekin.ai/feed", 60, true, null, null, null, "Last Week in AI", 3, null, 0 },
                    { 34, false, 0, "https://aiweekly.co/rss", 60, true, null, null, null, "AI Weekly", 2, null, 0 },
                    { 35, false, 0, "https://lilianweng.github.io/index.xml", 60, true, null, null, null, "Lilian Weng", 3, null, 0 },
                    { 36, false, 0, "https://magazine.sebastianraschka.com/feed", 60, true, null, null, null, "Sebastian Raschka", 3, null, 0 },
                    { 37, false, 0, "https://simonwillison.net/atom/everything/", 60, true, null, null, null, "Simon Willison", 3, null, 0 },
                    { 38, false, 0, "https://huyenchip.com/feed.xml", 60, true, null, null, null, "Chip Huyen", 3, null, 0 },
                    { 39, false, 0, "https://eugeneyan.com/writing/feed.xml", 60, true, null, null, null, "Eugene Yan", 3, null, 0 },
                    { 40, false, 0, "https://rss.arxiv.org/rss/cs.AI", 60, true, null, null, null, "ArXiv CS.AI", 3, null, 0 },
                    { 41, false, 0, "https://rss.arxiv.org/rss/cs.LG", 60, true, null, null, null, "ArXiv CS.LG", 3, null, 0 },
                    { 42, false, 0, "https://rss.arxiv.org/rss/cs.CL", 60, true, null, null, null, "ArXiv CS.CL", 3, null, 0 },
                    { 43, false, 0, "https://rss.arxiv.org/rss/cs.CV", 60, true, null, null, null, "ArXiv CS.CV", 3, null, 0 },
                    { 44, false, 0, "https://news.google.com/rss/search?q=%22Kimi+K2.6%22+OR+%22Moonshot+AI%22&hl=en-US&gl=US&ceid=US:en", 60, true, null, null, null, "Google News - Kimi K2.6", 3, null, 5 },
                    { 45, false, 0, "https://news.google.com/rss/search?q=%22Claude+Opus%22+OR+%22Claude+4.6%22+OR+%22Claude+4.7%22&hl=en-US&gl=US&ceid=US:en", 60, true, null, null, null, "Google News - Claude Opus", 3, null, 5 },
                    { 46, false, 0, "https://news.google.com/rss/search?q=%22Codex+5.5%22+OR+%22OpenAI+Codex%22&hl=en-US&gl=US&ceid=US:en", 60, true, null, null, null, "Google News - Codex", 3, null, 5 },
                    { 47, false, 0, "https://news.google.com/rss/search?q=%22GPT-5%22+OR+%22GPT-4.5%22+OR+%22OpenAI+o3%22&hl=en-US&gl=US&ceid=US:en", 60, true, null, null, null, "Google News - GPT-5", 3, null, 5 },
                    { 48, false, 0, "https://news.google.com/rss/search?q=%22Gemini+2.5%22+OR+%22Google+Gemini%22+model&hl=en-US&gl=US&ceid=US:en", 60, true, null, null, null, "Google News - Gemini", 3, null, 5 },
                    { 49, false, 0, "https://news.google.com/rss/search?q=%22large+language+model%22+OR+%22LLM%22+release+announcement&hl=en-US&gl=US&ceid=US:en", 60, true, null, null, null, "Google News - AI Models", 2, null, 5 },
                    { 50, false, 0, "https://www.reddit.com/r/LocalLLaMA/.json?limit=25", 60, true, null, null, null, "Reddit r/LocalLLaMA", 2, "Kimi,Claude,Codex,GPT,Gemini,Moonshot,Anthropic,OpenAI", 2 },
                    { 51, false, 0, "https://www.reddit.com/r/MachineLearning/.json?limit=25", 60, true, null, null, null, "Reddit r/MachineLearning", 2, "Kimi,Claude,Codex,GPT,Gemini,Moonshot,Anthropic,OpenAI", 2 },
                    { 52, false, 0, "https://www.reddit.com/r/singularity/.json?limit=25", 60, true, null, null, null, "Reddit r/singularity", 1, "Kimi,Claude,Codex,GPT,Gemini,Moonshot", 2 },
                    { 53, false, 0, "https://hn.algolia.com/api/v1/search?query=moonshot+ai+kimi&tags=story&hitsPerPage=30", 60, true, null, null, null, "HN - Kimi/Moonshot", 3, null, 3 },
                    { 54, false, 0, "https://hn.algolia.com/api/v1/search?query=claude+opus+anthropic&tags=story&hitsPerPage=30", 60, true, null, null, null, "HN - Claude", 3, null, 3 },
                    { 55, false, 0, "https://hn.algolia.com/api/v1/search?query=openai+codex+gpt&tags=story&hitsPerPage=30", 60, true, null, null, null, "HN - OpenAI/Codex", 3, null, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FeedSources_Priority",
                table: "FeedSources",
                column: "Priority");

            migrationBuilder.CreateIndex(
                name: "IX_FeedSources_Type",
                table: "FeedSources",
                column: "Type");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_ContentHash",
                table: "Articles",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_FetchedAt",
                table: "Articles",
                column: "FetchedAt");

            migrationBuilder.CreateIndex(
                name: "IX_Articles_MentionedModels",
                table: "Articles",
                column: "MentionedModels");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_FeedSources_Priority",
                table: "FeedSources");

            migrationBuilder.DropIndex(
                name: "IX_FeedSources_Type",
                table: "FeedSources");

            migrationBuilder.DropIndex(
                name: "IX_Articles_ContentHash",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_FetchedAt",
                table: "Articles");

            migrationBuilder.DropIndex(
                name: "IX_Articles_MentionedModels",
                table: "Articles");

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 30);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 31);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 32);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 33);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 34);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 35);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 36);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 37);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 38);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 39);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 40);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 41);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 42);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 43);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 44);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 45);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 46);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 47);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 48);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 49);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 50);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 51);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 52);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 53);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 54);

            migrationBuilder.DeleteData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 55);

            migrationBuilder.DropColumn(
                name: "AlwaysInclude",
                table: "FeedSources");

            migrationBuilder.DropColumn(
                name: "FetchIntervalMinutes",
                table: "FeedSources");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "FeedSources");

            migrationBuilder.DropColumn(
                name: "RequiredKeywords",
                table: "FeedSources");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "FeedSources");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MentionedCompanies",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "MentionedModels",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SearchVector",
                table: "Articles");

            migrationBuilder.DropColumn(
                name: "SourceType",
                table: "Articles");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "FeedSources",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "LastFetchStatus",
                table: "FeedSources",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "LastErrorMessage",
                table: "FeedSources",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "FeedUrl",
                table: "FeedSources",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Url",
                table: "Articles",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Title",
                table: "Articles",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Source",
                table: "Articles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "Author",
                table: "Articles",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.technologyreview.com/feed/", "MIT Technology Review" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 2,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.wired.com/feed/tag/ai/latest/rss", "Wired - AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 3,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.theverge.com/rss/ai-artificial-intelligence/index.xml", "The Verge - AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 4,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://venturebeat.com/category/ai/feed/", "VentureBeat - AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 5,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://techcrunch.com/category/artificial-intelligence/feed/", "TechCrunch - AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 6,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://arstechnica.com/tag/artificial-intelligence/feed/", "Ars Technica - AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 7,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://spectrum.ieee.org/rss/topic/artificial-intelligence", "IEEE Spectrum - AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 8,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://analyticsindiamag.com/feed/", "Analytics India Magazine" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 9,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.artificialintelligence-news.com/feed/", "AI News" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 10,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.marktechpost.com/feed/", "MarkTechPost" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 11,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.unite.ai/feed/", "Unite.AI" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 12,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://machinelearningmastery.com/feed/", "Machine Learning Mastery" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 13,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://towardsdatascience.com/feed", "Towards Data Science" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 14,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://ai.googleblog.com/feeds/posts/default", "Google AI Blog" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 15,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://openai.com/blog/rss.xml", "OpenAI Blog" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 16,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://deepmind.com/blog/feed/basic/", "DeepMind Blog" });

            migrationBuilder.UpdateData(
                table: "FeedSources",
                keyColumn: "Id",
                keyValue: 17,
                columns: new[] { "FeedUrl", "Name" },
                values: new object[] { "https://www.microsoft.com/en-us/research/blog/feed/", "Microsoft Research" });
        }
    }
}
