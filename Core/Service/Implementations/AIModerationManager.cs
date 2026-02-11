using Domain.Contracts;
using Domain.Entities.Posts;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Service.Specifications.ModerationSpecifications;
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
        private readonly IGenericRepository<AIModeration, int> moderationRepo;
        private readonly IContentModerationService _contentModeration;
        private readonly IUnitOfWork _unitOfWork;

        public AIModerationManager(
            IUnitOfWork unitOfWork,
            IContentModerationService contentModeration)
        {
            _unitOfWork = unitOfWork;
            _contentModeration = contentModeration;

            moderationRepo = _unitOfWork.GetRepository<AIModeration, int>();
        }

        public async Task<ModerationResult> ModerateAsync(string content, ModeratedEntityType entityType, int entityId)
        {
            if (string.IsNullOrWhiteSpace(content))
                return new ModerationResult { IsAccepted = true };

            var result = await _contentModeration.AnalyzeAsync(content);

            var spec = new ChangeEntityStatusSpecification(entityType, entityId);
            var entity = await moderationRepo.GetByIdAsync(spec);

            if (!result.IsAccepted)
            {
                if (entity == null)
                {
                    var moderation = new AIModeration
                    {
                        EntityType = entityType,
                        EntityId = entityId,
                        DetectedIssue = result.DetectedIssue,
                        ConfidenceScore = result.ConfidenceScore,
                        Status = ContentStatus.Denied
                    };

                    await moderationRepo.AddAsync(moderation);
                }
                else
                {
                    entity.DetectedIssue = result.DetectedIssue;
                    entity.ConfidenceScore = result.ConfidenceScore;
                    entity.ReviewedAt = DateTime.UtcNow;
                    moderationRepo.Update(entity);
                }
                    await _unitOfWork.SaveChangesAsync();
            }
            else
            {
                if(entity != null)
                {
                    moderationRepo.Delete(entity);
                    await _unitOfWork.SaveChangesAsync();
                }
            }

                return result;
        }
    }

}
