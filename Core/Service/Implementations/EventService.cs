using AutoMapper;
using Domain.Contracts;
using Domain.Entities.Engagement;
using Domain.Entities.Groups;
using Domain.Exceptions.EventExceptions;
using Domain.Exceptions.ForbiddenExceptions;
using Domain.Exceptions.GroupExceptions;
using Service.Specifications.EventSpecifications;
using ServiceAbstraction.Contracts;
using Shared.DTOs.EventModule;
using Shared.Enums;

namespace Service.Implementations
{
    public class EventService : IEventService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserGroupRelationService _relationService;
        private readonly INotificationService _notificationService;

        private readonly IGenericRepository<Event, int> eventRepo;
        private readonly IGenericRepository<EventParticipant, int> participantRepo;
        private readonly IGenericRepository<Group, int> groupRepo;

        public EventService(
            IMapper mapper,
            IUnitOfWork unitOfWork,
            IUserGroupRelationService relationService,
            INotificationService notificationService)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
            _relationService = relationService;
            _notificationService = notificationService;

            eventRepo = _unitOfWork.GetRepository<Event, int>();
            participantRepo = _unitOfWork.GetRepository<EventParticipant, int>();
            groupRepo = _unitOfWork.GetRepository<Group, int>();
        }

        public async Task<EventResponseDTO> CreateEventAsync(
        int groupId,
        string userId,
        CreateEventDTO dto)
        {
            if (!_relationService.IsMember(groupId))
                throw new ForbiddenActionException();

            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var @event = _mapper.Map<Event>(dto);

            @event.GroupId = groupId;
            @event.CreatedByUserId = userId;
            @event.CreatedAt = DateTime.UtcNow;
            @event.Status = EventStatus.Upcoming;

            await eventRepo.AddAsync(@event);
            await _unitOfWork.SaveChangesAsync();

            await participantRepo.AddAsync(new EventParticipant
            {
                EventId = @event.Id,
                UserId = userId,
                Status = EventResponseStatus.Going
            });

            await _unitOfWork.SaveChangesAsync();

            var spec = new EventByIdSpecification(@event.Id);

            var createdEvent = await eventRepo.GetByIdAsync(spec);

            return _mapper.Map<EventResponseDTO>(createdEvent);
        }



        public async Task<EventResponseDTO> GetEventByIdAsync(
        int eventId)
        {
            var spec = new EventByIdSpecification(eventId);

            var @event = await eventRepo.GetByIdAsync(spec)
                ?? throw new EventNotFoundException(eventId);

            return _mapper.Map<EventResponseDTO>(@event);
        }



        public async Task<List<EventResponseDTO>> GetGroupEventsAsync(
        int groupId)
        {
            var group = await groupRepo.GetByIdAsync(groupId)
                ?? throw new GroupNotFoundException(groupId);

            var spec = new GroupEventsSpecification(groupId);

            var events = await eventRepo.GetAllAsync(spec);

            return _mapper.Map<List<EventResponseDTO>>(events);
        }



        public async Task RespondToEventAsync(
        int eventId,
        string userId,
        EventResponseStatus status)
        {
            var spec = new EventByIdSpecification(eventId);

            var @event = await eventRepo.GetByIdAsync(spec)
                ?? throw new EventNotFoundException(eventId);

            if (@event.Status == EventStatus.Cancelled)
                throw new EventAlreadyCancelledException(eventId);

            if (@event.Status == EventStatus.Completed)
                throw new EventAlreadyCompletedException(eventId);

            if (!_relationService.IsMember(@event.GroupId))
                throw new ForbiddenActionException();

            var participantSpec =
                new EventParticipantSpecification(
                    eventId,
                    userId);

            var participant =
                await participantRepo.GetByIdAsync(participantSpec);

            if (participant is null)
            {
                participant = new EventParticipant
                {
                    EventId = eventId,
                    UserId = userId,
                    Status = status,
                    RespondedAt = DateTime.UtcNow
                };

                await participantRepo.AddAsync(participant);
            }
            else
            {
                participant.Status = status;
                participant.RespondedAt = DateTime.UtcNow;

                participantRepo.Update(participant);
            }

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task DeleteEventAsync(
        int eventId,
        string userId)
        {
            var spec = new EventByIdSpecification(eventId);

            var @event = await eventRepo.GetByIdAsync(spec)
                ?? throw new EventNotFoundException(eventId);

            if (@event.Status == EventStatus.Completed)
                throw new EventAlreadyCompletedException(eventId);

            if (!_relationService.IsAdmin(@event.GroupId)
                && !_relationService.IsOwner(@event.GroupId)
                && @event.CreatedByUserId != userId)
            {
                throw new ForbiddenActionException();
            }

            eventRepo.Delete(@event);

            await _unitOfWork.SaveChangesAsync();
        }


        public async Task<EventResponseDTO> UpdateEventAsync(
        int eventId,
        string userId,
        UpdateEventDTO dto)
        {
            var spec = new EventByIdSpecification(eventId);

            var @event = await eventRepo.GetByIdAsync(spec)
                ?? throw new EventNotFoundException(eventId);

            if (@event.Status == EventStatus.Cancelled)
                throw new EventAlreadyCancelledException(eventId);

            if (@event.Status == EventStatus.Completed)
                throw new EventAlreadyCompletedException(eventId);

            if (@event.CreatedByUserId != userId)
            {
                throw new ForbiddenActionException();
            }

            @event.Title = dto.Title;
            @event.Description = dto.Description;
            @event.Location = dto.Location;
            @event.EventDate = dto.EventDate;

            eventRepo.Update(@event);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<EventResponseDTO>(@event);
        }
    }
}
