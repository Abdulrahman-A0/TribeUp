using Domain.Contracts;
using Domain.Entities.Posts;
using ServiceAbstraction.Contracts;
using ServiceAbstraction.Models;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class AIModerationManager : IAIModerationManager
    {
        private readonly IContentModerationService _contentModeration;
        private readonly IUnitOfWork _unitOfWork;

        public AIModerationManager(
            IContentModerationService contentModeration,
            IUnitOfWork unitOfWork)
        {
            _contentModeration = contentModeration;
            _unitOfWork = unitOfWork;
        }

        public async Task<ModerationResult> ModerateAsync(string content, ModeratedEntityType entityType, int entityId)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new ModerationResult { IsAccepted = true };

            var result = await _contentModeration.AnalyzeAsync(content);

            if (!result.IsAccepted)
            {
                var moderation = new AIModeration
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    DetectedIssue = result.DetectedIssue,
                    ConfidenceScore = result.ConfidenceScore,
                    Status = ContentStatus.Denied
                };

                await _unitOfWork
                    .GetRepository<AIModeration, int>()
                    .AddAsync(moderation);

                await _unitOfWork.SaveChangesAsync();
            }

            return result;
        }
    }

}
