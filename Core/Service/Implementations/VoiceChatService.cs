using Livekit.Server.Sdk.Dotnet;
using Microsoft.Extensions.Options;
using ServiceAbstraction.Contracts;
using Shared.Common;
using Shared.DTOs.VirtualRoomModule;

namespace Service.Implementations
{
    public class VoiceChatService(IOptions<LiveKitOptions> options) : IVoiceChatService
    {
        public LiveKitTokenDTO GenerateToken(int groupId, string userId, string username)
        {
            var liveKitOptions = options.Value;

            var roomName = $"vr_room_{groupId}";

            var grants = new VideoGrants
            {
                RoomJoin = true,
                Room = roomName,
                CanPublish = true,
                CanSubscribe = true,
                CanPublishData = false
            };

            var token = new AccessToken(liveKitOptions.ApiKey, liveKitOptions.ApiSecret)
                .WithIdentity(userId)
                .WithName(username)
                .WithGrants(grants)
                .ToJwt();

            return new LiveKitTokenDTO(token, liveKitOptions.Url);
        }
    }
}
