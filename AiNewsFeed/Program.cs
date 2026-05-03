using Microsoft.EntityFrameworkCore;
using AiNewsFeed.Data;
using AiNewsFeed.Services;
using Scalar.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add controllers
builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// Register DbContext with SQL Server
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register feed fetcher service
builder.Services.AddScoped<IFeedFetcherService, FeedFetcherService>();

// Register HttpClient with User-Agent header
builder.Services.AddHttpClient("FeedClient", client =>
{
    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (compatible; AiNewsFeed/1.0)");
});

// CORS - allow all for localhost development
builder.Services.AddCors(options =>
{
    options.AddPolicy("DevCors", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.MapSwagger("/openapi/{documentName}.json");
    app.MapScalarApiReference();
}
// Middleware pipeline
app.UseHttpsRedirection();
app.UseCors("DevCors");
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

// Fallback to index.html for SPA behavior
app.MapFallbackToFile("index.html");

app.Run();