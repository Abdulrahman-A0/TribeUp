using Shared.DTOs.GroupMessages;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ServiceAbstraction.Contracts
{
    public interface IGroupChatService
    {
        Task<GroupMessageResponseDTO> SendMessageAsync(int groupId, SendGroupMessageDTO sendGroupMessageDTO, string userId);

        Task<PagedResult<GroupMessageResponseDTO>> GetMessagesAsync(int groupId, int page, int pageSize, string userId);
    }
}
