using Shared.DTOs.VirtualRoomModule;

namespace ServiceAbstraction.Contracts
{
    public interface IVoiceChatService
    {
        LiveKitTokenDTO GenerateToken(int groupId, string userId, string username);
    }
}
