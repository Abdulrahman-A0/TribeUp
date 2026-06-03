using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Engagement;
using Domain.Exceptions.Abstraction;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.PollExceptions;
using Domain.Exceptions.ValidationExceptions;
using Service.Specifications.PollSpecs;
using ServiceAbstraction.Contracts;
using Shared.DTOs.PollModule;
using Shared.DTOs.Posts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service.Implementations
{
    public class PollService : IPollService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserGroupRelationService _relationService;
        private readonly IMapper _mapper;

        public PollService(IUnitOfWork unitOfWork, IUserGroupRelationService relationService, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _relationService = relationService;
            _mapper = mapper;
        }

        public async Task<PagedResult<PollResultDTO>> GetGroupPollsAsync(int groupId, string userId, int page, int pageSize)
        {
            if (!_relationService.IsMember(groupId) && !_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var repo = _unitOfWork.GetRepository<Poll, int>();

            var spec = new GroupPollsSpecification(groupId, page, pageSize);
            var polls = await repo.GetAllAsync(spec);

            var countSpec = new GroupPollsCountSpecification(groupId);
            var totalCount = await repo.CountAsync(countSpec);

            var mappedPolls = _mapper.Map<List<PollResultDTO>>(polls, opts =>
            {
                opts.Items["UserId"] = userId;
                opts.Items["IncludeVoters"] = false;
            });

            return new PagedResult<PollResultDTO>
            {
                Items = mappedPolls,
                Page = page,
                PageSize = pageSize,
                TotalCount = totalCount,
                HasMore = totalCount > page * pageSize
            };
        }


        
        public async Task<PollResultDTO> GetPollResultsAsync(int pollId, string userId)
        {
            var repo = _unitOfWork.GetRepository<Poll, int>();

            var poll = await repo.GetByIdAsync(new PollFullDetailsSpecification(pollId))
                ?? throw new PollNotFoundException(pollId);

            if (!_relationService.IsMember(poll.GroupId) && !_relationService.IsAdmin(poll.GroupId) && !_relationService.IsOwner(poll.GroupId))
                throw new ForbiddenActionException();

            return _mapper.Map<PollResultDTO>(poll, opts =>
            {
                opts.Items["UserId"] = userId;
                opts.Items["IncludeVoters"] = true;
            });
        }

        
        public async Task<PollResultDTO> CreatePollAsync(int groupId, string userId, CreatePollDTO dto)
        {
            if (!_relationService.IsMember(groupId) && !_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
                throw new ForbiddenActionException();

            var validOptions = GetSanitizedOptions(dto.Options);

            if (validOptions.Count < 2)
            {
                throw new DomainValidationException(new Dictionary<string, string[]>
                {
                    ["Options"] = new[] { "Poll must have at least 2 unique and valid options." }
                });
            }

            var poll = new Poll
            {
                Question = dto.Question?.Trim() ?? string.Empty,
                ExpiresAt = dto.ExpiresAt,
                AllowMultipleAnswers = dto.AllowMultipleAnswers,
                CreatedByUserId = userId,
                GroupId = groupId,
                PollOptions = validOptions.Select(o => new PollOption { OptionText = o }).ToList()
            };

            var repo = _unitOfWork.GetRepository<Poll, int>();
            await repo.AddAsync(poll);
            await _unitOfWork.SaveChangesAsync();

            return await GetPollResultsAsync(poll.Id, userId);
        }



        public async Task<PollResultDTO> EditPollAsync(int pollId, string userId, EditPollDTO dto)
        {
            var pollRepo = _unitOfWork.GetRepository<Poll, int>();

            var poll = await pollRepo.GetByIdAsync(new PollWithOptionsSpecification(pollId))
                ?? throw new PollNotFoundException(pollId);

            if (poll.CreatedByUserId != userId)
            {
                throw new ForbiddenActionException();
            }

            if (!string.IsNullOrWhiteSpace(dto.Question))
            {
                poll.Question = dto.Question.Trim();
            }

            var validNewOptions = GetSanitizedOptions(dto.NewOptions);

            if (validNewOptions.Any())
            {
                AddUniqueOptionsToPoll(poll, validNewOptions);
            }

            pollRepo.Update(poll);
            await _unitOfWork.SaveChangesAsync();

            return await GetPollResultsAsync(poll.Id, userId);
        }



        public async Task<bool> DeletePollAsync(int pollId, string userId)
        {
            var pollRepo = _unitOfWork.GetRepository<Poll, int>();

            var poll = await pollRepo.GetByIdAsync(pollId)
                ?? throw new PollNotFoundException(pollId);
            

            bool isCreator = poll.CreatedByUserId == userId;
            bool isAdmin = _relationService.IsAdmin(poll.GroupId);
            bool isOwner = _relationService.IsOwner(poll.GroupId);

            if (!isCreator && !isAdmin && !isOwner)
            {
                throw new ForbiddenActionException();
            }
           
            pollRepo.Delete(poll);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }



        public async Task<ToggleVoteResultDTO> ToggleVoteAsync(int pollId, int optionId, string userId)
        {
            var pollRepo = _unitOfWork.GetRepository<Poll, int>();
            var voteRepo = _unitOfWork.GetRepository<PollVote, int>();

            var poll = await pollRepo.GetByIdAsync(new PollWithOptionsSpecification(pollId))
                ?? throw new PollNotFoundException(pollId);

            EnsureUserHasVotingPermissions(poll.GroupId);
            ValidatePollVotingEligibility(poll, optionId);

            var userVotesInThisPoll = (await voteRepo.GetAllAsync(new UserPollVotesSpecification(pollId, userId))).ToList();
            var specificVote = userVotesInThisPoll.FirstOrDefault(v => v.OptionId == optionId);

            if (specificVote != null)
            {
                return await RemoveVoteAsync(voteRepo, specificVote);
            }

            return await AddNewVoteAsync(voteRepo, poll, optionId, userId, userVotesInThisPoll);
        }




        #region Private Methods

        private List<string> GetSanitizedOptions(List<string>? rawOptions)
        {
            return rawOptions?
                .Where(o => !string.IsNullOrWhiteSpace(o))
                .Select(o => o.Trim())
                .Distinct()
                .ToList() ?? new List<string>();
        }

        private void AddUniqueOptionsToPoll(Poll poll, List<string> validNewOptions)
        {
            var existingOptionsSet = new HashSet<string>(
                poll.PollOptions.Select(o => o.OptionText),
                StringComparer.OrdinalIgnoreCase);

            var optionsToAdd = validNewOptions
                .Where(newOption => !existingOptionsSet.Contains(newOption))
                .Select(newOption => new PollOption { OptionText = newOption });

            foreach (var option in optionsToAdd)
            {
                poll.PollOptions.Add(option);
            }
        }


        private void EnsureUserHasVotingPermissions(int groupId)
        {
            if (!_relationService.IsMember(groupId) && !_relationService.IsAdmin(groupId) && !_relationService.IsOwner(groupId))
            {
                throw new ForbiddenActionException(); 
            }
        }

        private void ValidatePollVotingEligibility(Poll poll, int optionId)
        {
            if (poll.ExpiresAt.HasValue && poll.ExpiresAt.Value < DateTime.UtcNow)
            {
                throw new DomainValidationException(new Dictionary<string, string[]>
                {
                    ["Poll"] = new[] { "This poll has expired and no longer accepts votes." }
                });
            }

            if (!poll.PollOptions.Any(o => o.Id == optionId))
            {
                throw new DomainValidationException(new Dictionary<string, string[]>
                {
                    ["Option"] = new[] { "Invalid option for this poll." }
                });
            }
        }

        private async Task<ToggleVoteResultDTO> RemoveVoteAsync(IGenericRepository<PollVote, int> voteRepo,PollVote voteToRemove)
        {
            voteRepo.Delete(voteToRemove);
            await _unitOfWork.SaveChangesAsync();

            return new ToggleVoteResultDTO { Message = "Vote removed successfully.", IsVoted = false };
        }

        private async Task<ToggleVoteResultDTO> AddNewVoteAsync(
            IGenericRepository<PollVote, int> voteRepo,
            Poll poll,
            int optionId,
            string userId,
            List<PollVote> existingUserVotes)
        {
            if (!poll.AllowMultipleAnswers)
            {
                foreach (var oldVote in existingUserVotes)
                {
                    voteRepo.Delete(oldVote);
                }
            }

            var newVote = new PollVote { PollId = poll.Id, OptionId = optionId, UserId = userId };
            await voteRepo.AddAsync(newVote);
            await _unitOfWork.SaveChangesAsync();

            return new ToggleVoteResultDTO { Message = "Vote added successfully.", IsVoted = true };
        }

        #endregion
    }
}
