using Domain.Entities.Groups;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Users
{
    public class Recommendation : BaseEntity<int>
    {
        public string Reason { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        #region Relations
        public string UserId { get; set; }
        [ForeignKey(nameof(UserId))]
        public virtual ApplicationUser User { get; set; }

        public int SuggestedGroupID { get; set; }
        [ForeignKey(nameof(SuggestedGroupID))]
        public virtual Group SuggestedGroup { get; set; }
        #endregion
    }
}
