using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupScoreService
    {
        Task IncreaseOnJoinAsync(int groupId, int points = 10);
        Task DecreaseOnLeaveAsync(int groupId, int points = 10);
    }
}
