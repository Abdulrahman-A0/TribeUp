using AutoMapper;
using AutoMapper.Execution;
using Domain.Contracts;
using Domain.Entities.Groups;
using Domain.Exceptions.GroupExceptions;
using Service.Specifications.GroupChatMessageSpecs;
using Service.Specifications.GroupMemberSpecs;
using Service.Specifications.GroupSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.GroupMessageModule;
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
    public class GroupChatService(IUnitOfWork unitOfWork, IMapper mapper, IGroupAuthorizationService groupAuthorizationService, IGroupChatNotifier groupChatNotifier) : IGroupChatService
    {
        public async Task<GroupMessageResponseDTO> SendMessageAsync(int groupId, SendGroupMessageDTO sendGroupMessageDTO, string userId)
        {
            await groupAuthorizationService.EnsureUserCanChatAsync(groupId, userId);

            var messageRepo = unitOfWork.GetRepository<GroupChatMessage, long>();
            var groupRepo = unitOfWork.GetRepository<Group, int>();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var message = new GroupChatMessage
            {
                GroupId = groupId,
                UserId = userId,
                Content = sendGroupMessageDTO.Content,
                SentAt = DateTime.UtcNow
            };

            await messageRepo.AddAsync(message);

            group.LastMessage = message;
            group.LastMessageSentAt = message.SentAt;

            await unitOfWork.SaveChangesAsync(); // ✅ مرة واحدة

            var spec = new GroupChatMessageWithGroupSpec(message.Id);
            var savedMessage = await messageRepo.GetByIdAsync(spec);

            var response = mapper.Map<GroupMessageResponseDTO>(savedMessage);

            await groupChatNotifier.NotifyGroupAsync(groupId, response);

            return response;
        }




        public async Task<PagedResult<GroupMessageResponseDTO>> GetMessagesAsync(int groupId, int page, int pageSize, string userId)
        {
            await groupAuthorizationService.EnsureUserCanChatAsync(groupId, userId);

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



        public async Task<List<GroupChatInboxDTO>> GetChatInboxAsync(string userId)
        {
            var repo = unitOfWork.GetRepository<Group, int>();
            var spec = new ChatInboxProjectionSpec(userId);

            var groups = await repo.GetAllAsync(spec);

            return mapper.Map<List<GroupChatInboxDTO>>(groups);
        }



    }
}
