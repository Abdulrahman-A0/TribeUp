using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Users;
using Domain.Entities.VirtualRooms;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.UserExceptions;
using Microsoft.AspNetCore.Identity;
using Service.Specifications.VirtualRoomSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.VirtualRoomModule;

namespace Service.Implementations
{
    public class VirtualRoomService
        (
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserGroupRelationService groupRelationService,
        UserManager<ApplicationUser> userManager
        ) : IVirtualRoomService
    {
        public async Task<RoomDetailsDTO> JoinRoomAsync(int groupId, string userId)
        {
            if (!groupRelationService.IsMember(groupId))
                throw new ForbiddenActionException();

            var user = await userManager.FindByIdAsync(userId)
                 ?? throw new UserNotFoundException(userId);

            if (!string.IsNullOrWhiteSpace(user.Avatar))
            {
                var isAlive = await IsAvatarUrlAliveAsync(user.Avatar);
                if (!isAlive)
                {
                    user.Avatar = null;
                    await userManager.UpdateAsync(user);

                    throw new InvalidAvatarException();
                }
            }
            else
            {
                throw new InvalidAvatarException();
            }

            var roomRepo = unitOfWork.GetRepository<VirtualRoom, int>();
            var participantRepo = unitOfWork.GetRepository<RoomParticipant, int>();

            var spec = new GetRoomByGroupIdSpecification(groupId);

            var room = await roomRepo.GetByIdAsync(spec);

            if (room == null)
            {
                room = new VirtualRoom { GroupId = groupId, IsActive = true };
                await roomRepo.AddAsync(room);
                await unitOfWork.SaveChangesAsync();
            }

            if (!room.Participants.Any(p => p.UserId == userId))
            {
                var participant = new RoomParticipant
                {
                    VirtualRoomId = room.Id,
                    UserId = userId,
                };

                await participantRepo.AddAsync(participant);
                await unitOfWork.SaveChangesAsync();

                // Re-fetch the room to ensure EF Core eagerly loads the Navigation Property 
                // (.User) for the newly added participant so AutoMapper can map it correctly.
                room = await roomRepo.GetByIdAsync(spec);
            }

            var participantDtos = mapper.Map<IEnumerable<ParticipantDTO>>(room.Participants);

            return new RoomDetailsDTO(room.Id, room.GroupId, participantDtos);

        }

        public async Task LeaveRoomAsync(int groupId, string userId)
        {
            var roomRepo = unitOfWork.GetRepository<VirtualRoom, int>();
            var roomSpec = new GetRoomByGroupIdSpecification(groupId);
            var room = await roomRepo.GetByIdAsync(roomSpec);

            if (room == null) return;

            var participantRepo = unitOfWork.GetRepository<RoomParticipant, int>();
            var participantSpec = new GetParticipantSpecification(room.Id, userId);

            var participant = await participantRepo.GetByIdAsync(participantSpec);

            if (participant == null) return;

            participantRepo.Delete(participant);

            var remainingCount = room.Participants
                .Count(p => p.UserId != userId);

            if (remainingCount == 0)
            {
                roomRepo.Delete(room);
            }

            await unitOfWork.SaveChangesAsync();
        }

        public async Task<IEnumerable<ParticipantDTO>> GetActiveParticipantsAsync(int groupId)
        {
            if (!groupRelationService.IsMember(groupId))
                throw new ForbiddenActionException();

            var spec = new GetRoomByGroupIdSpecification(groupId);
            var room = await unitOfWork.GetRepository<VirtualRoom, int>()
                .GetByIdAsync(spec);

            if (room == null) return Enumerable.Empty<ParticipantDTO>();

            return mapper.Map<IEnumerable<ParticipantDTO>>(room.Participants);
        }


        private static async Task<bool> IsAvatarUrlAliveAsync(string url)
        {
            try
            {
                using var http = new HttpClient();
                http.Timeout = TimeSpan.FromSeconds(5);

                var response = await http.SendAsync(
                    new HttpRequestMessage(HttpMethod.Get, url),
                    HttpCompletionOption.ResponseHeadersRead);

                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
