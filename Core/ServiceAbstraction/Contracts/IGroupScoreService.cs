using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupScoreService
    {
        Task IncreaseOnActionAsync(int groupId, int points = 10);
        Task DecreaseOnActionAsync(int groupId, int points = 10);
    }
}
