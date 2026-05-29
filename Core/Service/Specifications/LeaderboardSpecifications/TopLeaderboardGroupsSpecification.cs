using Domain.Entities.Groups;

namespace Service.Specifications.GroupSpecs;

public class TopLeaderboardGroupsSpecification
    : BaseSpecifications<GroupScore,int>
{
    public TopLeaderboardGroupsSpecification(int top)
        : base(g => true)
    {
        AddIncludes(g => g.Group);

        AddOrderByDescending(g => g.TotalPoints);

        ApplyPagination(1, top);
    }
}