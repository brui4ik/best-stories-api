namespace BestStoriesApi.HackerNews.Dtos;

public sealed record HackerNewsItem(
    string By,
    int Descendants,
    long Id,
    long[]? Kids,
    int Score,
    long Time,
    string Title,
    string Type,
    string Url
);