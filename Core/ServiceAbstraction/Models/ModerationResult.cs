using Shared.Enums;

namespace ServiceAbstraction.Models
{
    public class ModerationResult
    {
        public string DetectedIssue { get; set; } = string.Empty;
        public double ConfidenceScore { get; set; }
        public ContentStatus Status { get; set; }
    }

}
