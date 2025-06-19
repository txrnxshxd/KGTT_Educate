using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;
using KGTT_Educate.Services.Events.Models.Dto;
using KGTT_Educate.Services.Events.SyncDataServices.Grpc;
using Mapster;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations.Schema;

namespace KGTT_Educate.Services.Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IUoW _uow;
        private readonly IConfiguration _configuration;

        public EventsController(IUoW uow, IConfiguration configuration)
        {
            _configuration = configuration;
            _uow = uow;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Event> events = _uow.Events.GetAll().OrderByDescending(x => x.Date);

            if (!events.Any()) return NotFound("Не найдено ни одного объекта");

            return Ok(events);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "Authenticated")]
        public IActionResult GetById(Guid id)
        {
            Event evnt = _uow.Events.Get(x => x.Id == id);

            return Ok(evnt);
        }


        [HttpPost]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Post(EventDTO evnt)
        {
            if (evnt == null) return BadRequest();

            _uow.Events.Add(evnt.Adapt<Event>());

            _uow.Save();

            return Ok(evnt);
        }

        [HttpPut]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Edit([FromForm] Event evnt)
        {
            if (evnt == null) return BadRequest();

            _uow.Events.Update(evnt);

            _uow.Save();

            return Ok(evnt);
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult Delete(Guid id)
        {
            Event evnt = _uow.Events.Get(x => x.Id == id);

            if (evnt == null) return NotFound();
            
            _uow.Events.Delete(evnt);

            _uow.Save();

            return Ok(evnt);
        }

        // Пользователи мероприятия
        [HttpGet("Users/{eventId}")]
        [Authorize(Policy = "Authenticated")]
        public IActionResult GetEventUsers(Guid eventId)
        {
            IEnumerable<EventGroup> groups = _uow.EventGroup.GetMany(x => x.EventId == eventId, "Event").Distinct();

            if (groups == null || !groups.Any()) return NotFound();

            List<UserGroupDTO> users = new();

            var grpcClient = new GrpcAccountClient(_configuration);

            foreach (var group in groups)
            {
                var userGroup = grpcClient.GetUserGroup(group.GroupId);

                foreach (var user in userGroup)
                {
                    users.Add(user);
                }
            }

            if (!users.Any()) return NotFound();

            return Ok(users);
        }

        [HttpGet("Groups/{eventId}")]
        [Authorize(Policy = "Authenticated")]
        public IActionResult GetEventGroups(Guid eventId)
        {
            IEnumerable<EventGroup> groups = _uow.EventGroup.GetMany(x => x.EventId == eventId, "Event").Distinct();

            if (groups == null || !groups.Any()) return NotFound("Групп не найдено");

            return Ok(groups);
        }

        [HttpPost("Group/{groupId}/Event/{eventId}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult CreateEventGroup(Guid eventId, Guid groupId)
        {
            Event evnt = _uow.Events.Get(x => x.Id == eventId);

            var grpcClient = new GrpcAccountClient(_configuration);
            GroupDTO group = grpcClient.GetGroup(groupId);

            if (group == null) return NotFound();

            EventGroup eventUser = new()
            {
                GroupId = group.Id,
                EventId = eventId,
                Event = evnt
            };

            _uow.EventGroup.Add(eventUser);

            _uow.Save();

            return Ok();
        }

        [HttpDelete("Group/{groupId}/Event/{eventId}")]
        [Authorize(Policy = "AdminOnly")]
        public IActionResult DeleteEventGroup(Guid eventId, Guid groupId)
        {
            var grpcClient = new GrpcAccountClient(_configuration);
            GroupDTO group = grpcClient.GetGroup(groupId);

            if (group == null) return NotFound();

            EventGroup eventGroup = _uow.EventGroup.Get(x => x.GroupId == groupId && x.EventId == eventId);

            _uow.EventGroup.Delete(eventGroup);

            _uow.Save();

            return Ok();
        }
    }
}
