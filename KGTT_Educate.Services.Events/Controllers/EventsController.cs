using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;
using KGTT_Educate.Services.Events.Models.Dto;
using KGTT_Educate.Services.Events.SyncDataServices.Grpc;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

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
        public IActionResult GetById(Guid id)
        {
            Event evnt = _uow.Events.Get(x => x.Id == id);

            return Ok(evnt);
        }


        [HttpPost]
        public IActionResult Post([FromForm] EventDTO evnt)
        {
            if (evnt == null) return BadRequest();

            _uow.Events.Add(evnt.Adapt<Event>());

            _uow.Save();

            return Ok(evnt);
        }

        [HttpPut]
        public IActionResult Edit([FromForm] EventDTO evnt)
        {
            if (evnt == null) return BadRequest();

            _uow.Events.Update(evnt.Adapt<Event>());

            _uow.Save();

            return Ok(evnt);
        }

        [HttpDelete("{id}")]
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
        public IActionResult GetEventUsers(Guid eventId)
        {
            IEnumerable<EventUser> users = _uow.EventUser.GetMany(x => x.EventId == eventId, "Event");

            if (!users.Any()) return NotFound();

            return Ok(users);
        }

        // Получение пользователей по 
        [HttpGet("Group/{groupId}")]
        public IActionResult GetEventGroupUsers(Guid groupId)
        {
            var grpcClient = new GrpcAccountClient(_configuration);
            IEnumerable<UserGroupDTO> users = grpcClient.GetUserGroup(groupId);

            if (users == null || users.Count() == 0) return NotFound();

            return Ok(users);
        }

        [HttpPost("Group/{groupId}")]
        public IActionResult CreateEventGroup(Guid eventId, Guid groupId)
        {
            Event evnt = _uow.Events.Get(x => x.Id == eventId);

            var grpcClient = new GrpcAccountClient(_configuration);
            IEnumerable<UserGroupDTO> userGroup = grpcClient.GetUserGroup(groupId);

            if (userGroup == null) return NotFound();

            foreach (var user in userGroup)
            {
                EventUser eventUser = new()
                {
                    UserId = user.User.Id,
                    EventId = eventId,
                    Event = evnt
                };

                _uow.EventUser.Add(eventUser);
            }

            _uow.Save();

            return Ok();
        }
    }
}
