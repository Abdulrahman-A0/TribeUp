using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Groups
{
    public class Badge : BaseEntity<int>
    {
        public string BadgeName { get; set; }
        public int PointsEarned { get; set; } = 0;
        public DateTime AchievedAt { get; set; } = DateTime.Now;


        #region Navigation properties
        public int GroupId { get; set; }

        [ForeignKey(nameof(GroupId))]
        public virtual Group Group { get; set; }

        #endregion
    }
}
