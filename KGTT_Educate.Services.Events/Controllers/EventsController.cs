using KGTT_Educate.Services.Events.Data.Repository.Interfaces;
using KGTT_Educate.Services.Events.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace KGTT_Educate.Services.Events.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EventsController : ControllerBase
    {
        private readonly IUoW _uow;

        public EventsController(IUoW uow)
        {
            _uow = uow;
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            IEnumerable<Event> events = _uow.Events.GetAll();

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
        public IActionResult Post([FromForm] Event evnt)
        {
            if (evnt == null) return BadRequest();

            _uow.Events.Add(evnt);

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
    }
}
