using Shared.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Posts
{
    public class AIModeration : BaseEntity<int>
    {
        public ModeratedEntityType EntityType { get; set; }
        public int EntityId { get; set; }

        public string DetectedIssue { get; set; } = string.Empty;
        public double? ConfidenceScore { get; set; }
        public ContentStatus Status { get; set; } = ContentStatus.Accepted;
        public DateTime ReviewedAt { get; set; } = DateTime.UtcNow;
 

    }
}
