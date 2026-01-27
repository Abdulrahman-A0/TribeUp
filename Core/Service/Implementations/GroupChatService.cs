using AutoMapper;
using AutoMapper.Execution;
using Domain.Contracts;
using Domain.Entities.Groups;
using Service.Specifications.GroupChatMessageSpecs;
using Service.Specifications.GroupMemberSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMessages;
using Shared.DTOs.Posts;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class GroupChatService(IUnitOfWork unitOfWork, IMapper mapper, IGroupChatPermissionService groupChatPermissionService, IGroupChatNotifier groupChatNotifier) : IGroupChatService
    {
        public async Task<GroupMessageResponseDTO> SendMessageAsync(int groupId, SendGroupMessageDTO sendGroupMessageDTO, string userId)
        {
            await groupChatPermissionService.EnsureUserCanChatAsync(groupId, userId);

            var message = new GroupChatMessage
            {
                GroupId = groupId,
                UserId = userId,
                Content = sendGroupMessageDTO.Content,
                SentAt = DateTime.UtcNow
            };

            var repo = unitOfWork.GetRepository<GroupChatMessage, long>();
            await repo.AddAsync(message);
            await unitOfWork.SaveChangesAsync();

            var spec = new GroupChatMessageWithUserSpec(message.Id);
            var savedMessage = await repo.GetByIdAsync(spec);

            var response = mapper.Map<GroupMessageResponseDTO>(savedMessage);

            await groupChatNotifier.NotifyGroupAsync(groupId, response);

            return response;
        }




        public async Task<PagedResult<GroupMessageResponseDTO>> GetMessagesAsync(int groupId, int page, int pageSize, string userId)
        {
            await groupChatPermissionService.EnsureUserCanChatAsync(groupId, userId);

            var repo = unitOfWork.GetRepository<GroupChatMessage, long>();
            var spec = new GroupChatMessagesSpec(groupId, page, pageSize);
            var messages = await repo.GetAllAsync(spec);

            var totalCount = await repo.CountAsync(m => m.GroupId == groupId && !m.IsDeleted);

            var mappedMessages = mapper.Map<List<GroupMessageResponseDTO>>(messages);

            return new PagedResult<GroupMessageResponseDTO>
            {
                Items = mappedMessages,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = (page * pageSize) < totalCount
            };
        }



        
    }
}
