namespace Domain.Entities.Users
{
    public class RefreshToken : BaseEntity<Guid>
    {
        public string TokenHash { get; set; } = null!;
        public DateTime ExpiresAt { get; set; }
        public bool IsRevoked { get; set; }
        public string DeviceId { get; set; } = null!;

        #region Relations
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;
        #endregion
    }

}
