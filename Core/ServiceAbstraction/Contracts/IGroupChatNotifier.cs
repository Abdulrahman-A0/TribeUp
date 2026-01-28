using Shared.DTOs.GroupMessages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupChatNotifier
    {
        Task NotifyGroupAsync(int groupId, GroupMessageResponseDTO message);
    }
}
