using Domain.Entities.Groups;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities.Engagement
{
    public class Event : BaseEntity<int>
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime EventDate { get; set; }
        public string Location { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;


        #region Relations
        public int GroupId { get; set; }
        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }
        #endregion
    }
}
