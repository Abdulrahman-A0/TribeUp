using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ServiceAbstraction.Contracts;
using Shared.DTOs; // مسار الـ PagedResult
using Shared.DTOs.PollModule;
using Shared.DTOs.Posts;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Presentation.Controller
{
    [Authorize]
    public class PollsController(IServiceManager service) : ApiController
    {
        /// <summary>
        /// Creates a new poll inside a group.
        /// </summary>
        /// <remarks>
        /// The poll must have at least two unique options. 
        /// 
        /// If AllowMultipleAnswers is true, users can vote for more than one option.
        /// If false, voting for a new option will automatically remove the user's previous vote (like radio buttons).
        /// 
        /// Example:
        /// 
        /// {
        ///   "question": "What is the best architecture pattern?",
        ///   "expiresAt": "2026-12-31T23:59:59",
        ///   "allowMultipleAnswers": false,
        ///   "options": [
        ///     "Clean Architecture",
        ///     "Monolithic",
        ///     "Microservices"
        ///   ]
        /// }
        /// </remarks>
        /// <param name="groupId">The target group identifier.</param>
        /// <param name="dto">The poll information and options.</param>
        /// <response code="200">Poll created successfully.</response>
        /// <response code="400">Validation failed (e.g., less than 2 options).</response>
        /// <response code="403">User is not a member of the group.</response>
        /// <response code="404">Group not found.</response>
        [HttpPost("{groupId:int}/CreatePoll")]
        public async Task<ActionResult<PollResultDTO>> CreatePoll(
            int groupId,
            [FromBody] CreatePollDTO dto)
            => Ok(await service.PollService.CreatePollAsync(groupId, UserId, dto));


        /// <summary>
        /// Retrieves all polls belonging to a specific group with pagination.
        /// </summary>
        /// <remarks>
        /// This endpoint is optimized for feeds. 
        /// It returns poll details, options, vote counts, and percentages, 
        /// but intentionally excludes detailed voter profiles (images/names) to save bandwidth.
        /// </remarks>
        /// <param name="groupId">The target group identifier.</param>
        /// <param name="page">The page number (default is 1).</param>
        /// <param name="pageSize">The number of items per page (default is 10).</param>
        /// <response code="200">Polls retrieved successfully.</response>
        /// <response code="403">User is not a member of the group.</response>
        /// <response code="404">Group not found.</response>
        [HttpGet("{groupId:int}/GroupPolls")]
        public async Task<ActionResult<PagedResult<PollResultDTO>>> GetGroupPolls(
            int groupId,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
            => Ok(await service.PollService.GetGroupPollsAsync(groupId, UserId, page, pageSize));


        /// <summary>
        /// Retrieves detailed information about a specific poll.
        /// </summary>
        /// <remarks>
        /// The response includes:
        /// - Poll question and status
        /// - Total unique voters count
        /// - Each option's vote count and percentage
        /// - Detailed list of voters (names, profile pictures, voting time) for each option.
        /// 
        /// This endpoint is typically used when opening the poll details view.
        /// </remarks>
        /// <param name="pollId">The target poll identifier.</param>
        /// <response code="200">Poll retrieved successfully.</response>
        /// <response code="403">User is not a member of the group.</response>
        /// <response code="404">Poll not found.</response>
        [HttpGet("{pollId:int}/GetPollById")]
        public async Task<ActionResult<PollResultDTO>> GetPollById(
            int pollId)
            => Ok(await service.PollService.GetPollResultsAsync(pollId, UserId));


        /// <summary>
        /// Updates an existing poll (Creator Only).
        /// </summary>
        /// <remarks>
        /// Only the poll creator can update the poll.
        /// 
        /// Editable fields:
        /// - Question text
        /// - Adding new options (NewOptions array)
        /// 
        /// Note: Existing options cannot be removed to preserve voting integrity.
        /// New options will be ignored if they already exist.
        /// </remarks>
        /// <param name="pollId">The target poll identifier.</param>
        /// <param name="dto">The updated question or new options to add.</param>
        /// <response code="200">Poll updated successfully.</response>
        /// <response code="403">User is not the creator of this poll.</response>
        /// <response code="404">Poll not found.</response>
        [HttpPut("{pollId:int}/UpdatePoll")]
        public async Task<ActionResult<PollResultDTO>> UpdatePoll(
            int pollId,
            [FromBody] EditPollDTO dto)
            => Ok(await service.PollService.EditPollAsync(pollId, UserId, dto));


        /// <summary>
        /// Deletes an existing poll.
        /// </summary>
        /// <remarks>
        /// Only the following users may delete a poll:
        /// - Poll creator
        /// - Group Admin
        /// - Group Owner
        /// 
        /// When a poll is deleted, all related options and user votes are permanently removed.
        /// This action cannot be undone.
        /// </remarks>
        /// <param name="pollId">The target poll identifier.</param>
        /// <response code="200">Poll deleted successfully.</response>
        /// <response code="403">User lacks permission to delete this poll.</response>
        /// <response code="404">Poll not found.</response>
        [HttpDelete("{pollId:int}/DeletePoll")]
        public async Task<IActionResult> DeletePoll(
            int pollId)
        {
            await service.PollService.DeletePollAsync(pollId, UserId);
            return Ok(new { Message = "Poll deleted successfully." });
        }


        /// <summary>
        /// Toggles a user's vote for a specific poll option.
        /// </summary>
        /// <remarks>
        /// Smart Toggle Behavior:
        /// - Toggle On: If the user hasn't voted for this option, a vote is added.
        /// - Toggle Off: If the user clicks an option they already voted for, the vote is removed.
        /// 
        /// Rule for Single-Answer Polls (AllowMultipleAnswers = false):
        /// - Voting for a new option will automatically erase any existing vote the user has in this poll.
        /// </remarks>
        /// <param name="pollId">The target poll identifier.</param>
        /// <param name="optionId">The target option identifier.</param>
        /// <response code="200">Vote toggled successfully (returns whether it was added or removed).</response>
        /// <response code="400">Poll is expired or the option is invalid.</response>
        /// <response code="403">User is not a member of the group.</response>
        /// <response code="404">Poll not found.</response>
        [HttpPost("{pollId:int}/Options/{optionId:int}/ToggleVote")]
        public async Task<ActionResult<ToggleVoteResultDTO>> ToggleVote(
            int pollId,
            int optionId)
            => Ok(await service.PollService.ToggleVoteAsync(pollId, optionId, UserId));
    }
}