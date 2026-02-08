using BestStoriesApi.HackerNews.Dtos;
using BestStoriesApi.HackerNews.Models;
using MediatR;

namespace BestStoriesApi.HackerNews.Queries;

public sealed record GetBestStoriesQuery(int N)
    : IRequest<List<BestStory>>;