using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
using ServiceAbstraction.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class GroupScoreService(IUnitOfWork unitOfWork) : IGroupScoreService
    {
        public async Task IncreaseOnActionAsync(int groupId, int points = 10)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId);

            group.GroupScore ??= new GroupScore
            {
                GroupId = groupId
            };

            group.GroupScore.TotalPoints += points;
            group.GroupScore.LastUpdated = DateTime.UtcNow;

            groupRepo.Update(group);
        }


        public async Task DecreaseOnActionAsync(int groupId, int points = 10)
        {
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId);

            group.GroupScore ??= new GroupScore
            {
                GroupId = groupId
            };

            group.GroupScore.TotalPoints = Math.Max(0, group.GroupScore.TotalPoints - points);

            group.GroupScore.LastUpdated = DateTime.UtcNow;

            groupRepo.Update(group);
        }

    }
}
