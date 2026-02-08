using System.Text.Json.Serialization;

namespace BestStoriesApi.HackerNews.Models;

public sealed class BestStory
{
    [JsonPropertyName("title")] public string? Title { get; set; }
    [JsonPropertyName("uri")] public string? Uri { get; set; }
    [JsonPropertyName("postedBy")] public string? PostedBy { get; set; }
    [JsonPropertyName("time")] public DateTimeOffset? Time { get; set; }
    [JsonPropertyName("score")] public int? Score { get; set; }
    [JsonPropertyName("commentCount")] public int? CommentCount { get; set; }
}