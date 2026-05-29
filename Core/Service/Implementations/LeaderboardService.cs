using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GamificationExceptions;
using Service.Specifications.GroupSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.LeaderboardModule;

namespace Service.Implementations;

public class LeaderboardService : ILeaderboardService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public LeaderboardService(
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<IEnumerable<LeaderboardGroupDTO>>GetTopGroupsAsync(int top)
    {
        if (top < 1 || top > 100)
            throw new InvalidLeaderboardRangeException();

        var spec = new TopLeaderboardGroupsSpecification(top);

        var leaderboard =
            await _unitOfWork
                .GetRepository<GroupScore,int>()
                .GetAllAsync(spec);

        var mapped =
            _mapper.Map<IEnumerable<LeaderboardGroupDTO>>(leaderboard)
                .ToList();

        AssignBadges(mapped);

        return mapped;
    }

    private void AssignBadges(
        List<LeaderboardGroupDTO> groups)
    {
        for (int i = 0; i < groups.Count; i++)
        {
            groups[i].Rank = i + 1;

            switch (i + 1)
            {
                case 1:
                    groups[i].BadgeName = "Legend";
                    groups[i].BadgeIcon = "legend.png";
                    break;

                case 2:
                    groups[i].BadgeName = "Master";
                    groups[i].BadgeIcon = "master.png";
                    break;

                case 3:
                    groups[i].BadgeName = "Elite";
                    groups[i].BadgeIcon = "elite.png";
                    break;

                default:
                    groups[i].BadgeName = "Top Group";
                    groups[i].BadgeIcon = "default.png";
                    break;
            }
        }
    }
}