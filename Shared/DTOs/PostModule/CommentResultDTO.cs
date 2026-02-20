using Microsoft.Extensions.Hosting;
using Shared.DTOs.CommentModule;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs.PostModule
{
    public class CommentResultDTO
    {
        public string UserId { get; set; } = null!;
        public string Username { get; set; } = null!;
        public string ProfilePicture { get; set; } = null!;
        public int Id { get; set; }
        public string Content { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public int PostId { get; set; }
        public bool IsLikedByCurrentUser { get; set; }
        public int LikesCount { get; set; }
        public CommentPermissionDTO Permissions { get; set; }= new();

    }
}
