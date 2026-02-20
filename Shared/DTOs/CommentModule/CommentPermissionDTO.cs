using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.CommentModule
{
    public class CommentPermissionDTO
    {
        public bool CanDelete { get; set; }
        public bool CanEdit { get; set; }
        public bool CanModerate { get; set; }
    }
}
