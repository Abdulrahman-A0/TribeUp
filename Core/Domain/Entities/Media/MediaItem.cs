using Domain.Entities.Posts;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Entities.Media
{
    public class MediaItem : BaseEntity<int>
    {
        public string MediaURL { get; set; }
        public string Type { get; set; }
        public DateTime UploadedAt { get; set; } = DateTime.Now;
        public int Order { get; set; } = 0;

        #region Navigation properties
        public int? PostId { get; set; }
        [ForeignKey(nameof(PostId))]
        public virtual Post Post { get; set; }
        public int? AlbumId { get; set; }
        [ForeignKey(nameof(AlbumId))]
        public virtual Album Album { get; set; }
        #endregion
    }
}
