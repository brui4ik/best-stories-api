using AutoMapper;
using BestStoriesApi.HackerNews.Dtos;
using BestStoriesApi.HackerNews.Models;

namespace BestStoriesApi.HackerNews.Mapping;

public class BestStoryProfile : Profile
{
    public BestStoryProfile()
    {
        CreateMap<HackerNewsItem, BestStory>()
            .ForMember(d => d.Title, o => o.MapFrom(s => s.Title))
            .ForMember(d => d.Uri, o => o.MapFrom(s => s.Url))
            .ForMember(d => d.PostedBy, o => o.MapFrom(s => s.By))
            .ForMember(d => d.Time, o => o.MapFrom(s => DateTimeOffset.FromUnixTimeSeconds(s.Time)))
            .ForMember(d => d.Score, o => o.MapFrom(s => s.Score))
            .ForMember(d => d.CommentCount, o => o.MapFrom(s => s.Descendants));
    }
}