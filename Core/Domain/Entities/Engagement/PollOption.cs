namespace Domain.Entities.Engagement
{
    public class PollOption : BaseEntity<int>
    {
        public string OptionText { get; set; }
        public int VotesCount { get; set; } = 0;

        #region Relations
        public int PollId { get; set; }
        public virtual Poll Poll { get; set; }

        public virtual ICollection<PollVote> PollVotes { get; set; } = new List<PollVote>();
        #endregion
    }
}
