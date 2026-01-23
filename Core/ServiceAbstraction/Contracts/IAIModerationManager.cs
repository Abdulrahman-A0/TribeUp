using ServiceAbstraction.Models;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IAIModerationManager
    {
        Task<ModerationResult> ModerateAsync(string content, ModeratedEntityType entityType, int entityId);
    }
}
