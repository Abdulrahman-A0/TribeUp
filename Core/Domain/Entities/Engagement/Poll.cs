using Domain.Entities.Groups;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Engagement
{
    public class Poll : BaseEntity<int>
    {
        public string Question { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        public DateTime? ExpiresAt { get; set; }

        #region Relations
        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }

        public ICollection<PollOption> PollOptions { get; set; } = new List<PollOption>();
        #endregion
    }
}
