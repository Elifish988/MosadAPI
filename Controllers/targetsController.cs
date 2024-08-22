using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Helper;
using MosadApi.Meneger;
using MosadApi.Models;
using MosadApi.Models;

namespace MosadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class targetsController : ControllerBase
    {
        
        
        private readonly MosadDBContext _context;
        private readonly CreatMissionByTarget _creatMissionByTarget;
        public targetsController(MosadDBContext mosadDBContext, CreatMissionByTarget creatMissionByTarget)
        {
            _context = mosadDBContext;
            _creatMissionByTarget = creatMissionByTarget;
        }

        [HttpGet]
        public async Task<IActionResult> Gettargets()
        {
            var target = await _context.agents.ToListAsync();
            return Ok(target);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateTarget(Target target)
        {
            target.Id = Guid.NewGuid();
            target.Status = StatusTarget.free;
            _context.targets.Add(target);
            _context.SaveChanges();
            return Ok(target.Id);
        }

        [HttpPut("{id}/pin")]
        public async Task<IActionResult> PutAgent(Guid id, Location location)
        {
            Target? target = await _context.targets.FindAsync(id);
            if (target == null) { return StatusCode(StatusCodes.Status404NotFound); }
            else
            {
                _context.locations.Add(location);
                target.Location = location;
                target.LocationId = location.Id;
                _context.SaveChanges();
                await _creatMissionByTarget.SearchTargetToTarget(target);// בדיקה האם יש מטרה קרובה
                _creatMissionByTarget.DeleteOldTasks();// מחיקת הצעות לא רלוונטיות
                return Ok(target);
            }
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> PutAgent(Guid id, Direction direction)
        {
            Target? target = await _context.targets.FindAsync(id);
            if (target == null) { return NotFound("agent is null"); }
            else
            {
                Location? location = await _context.locations.FindAsync(target.LocationId);
                location = LoctionMeneger.ChangeLocation(location, direction);
                _context.Update(location);
                _context.SaveChanges();
                await _creatMissionByTarget.SearchTargetToTarget(target);// בדיקה האם יש מטרה קרובה
                _creatMissionByTarget.DeleteOldTasks();// מחיקת הצעות לא רלוונטיות
                return Ok();

            }
        }
    }
}
