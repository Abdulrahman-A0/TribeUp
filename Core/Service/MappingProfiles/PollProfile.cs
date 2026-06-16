using AutoMapper;
using Domain.Entities.Engagement;
using Service.MappingProfiles.MediaResolvers;
using Shared.DTOs.PollModule;

namespace Service.MappingProfiles
{
    public class PollProfile : Profile
    {
        public PollProfile()
        {
            CreateMap<PollVote, VoterDTO>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.User.UserName))
                .ForMember(dest => dest.ProfilePictureUrl, opt => opt.MapFrom<UserProfilePictureResolver, string>(src => src.User.ProfilePicture!))
                .ForMember(dest => dest.VotedAt, opt => opt.MapFrom(src => src.VotedAt));





            CreateMap<PollOption, PollOptionResultDTO>()
                .ForMember(dest => dest.OptionId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.VotesCount, opt => opt.MapFrom(src => src.PollVotes.Count))
                .ForMember(dest => dest.Voters, opt => opt.MapFrom(src => src.PollVotes.OrderByDescending(v => v.VotedAt)));




            CreateMap<Poll, PollResultDTO>()
                .ForMember(dest => dest.PollId, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedByUserName, opt => opt.MapFrom(src => src.CreatedByUser.UserName))
                .ForMember(dest => dest.IsExpired, opt => opt.MapFrom(src => src.ExpiresAt.HasValue && src.ExpiresAt.Value < DateTime.UtcNow))
                .ForMember(dest => dest.Options, opt => opt.MapFrom(src => src.PollOptions))

                .AfterMap((src, dest, context) =>
                {
                    var totalVoters = src.PollOptions.SelectMany(o => o.PollVotes).Select(v => v.UserId).Distinct().Count();
                    dest.TotalUniqueVoters = totalVoters;

                    string? currentUserId = context.Items.TryGetValue("UserId", out var id) ? id.ToString() : null;
                    bool includeVoters = context.Items.TryGetValue("IncludeVoters", out var inc) && (bool)inc;

                    foreach (var optionDto in dest.Options)
                    {
                        var originalOption = src.PollOptions.First(o => o.Id == optionDto.OptionId);

                        optionDto.Percentage = totalVoters == 0 ? 0 : Math.Round(((double)originalOption.PollVotes.Count / totalVoters) * 100, 2);

                        if (currentUserId != null)
                            optionDto.IsVotedByCurrentUser = originalOption.PollVotes.Any(v => v.UserId == currentUserId);

                        if (!includeVoters)
                            optionDto.Voters = null;
                    }
                });
        }
    }
}