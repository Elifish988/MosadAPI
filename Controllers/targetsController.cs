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
    [Route("[controller]")]
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
            var target = await _context.targets.ToListAsync();
            return Ok(target);
        }

        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateTarget(Target target)
        {
            target.Status = StatusTarget.free;
            _context.targets.Add(target);
            _context.SaveChanges();
            return StatusCode(StatusCodes.Status200OK, new { Id = target.Id });
        }

        [HttpPut("{id}/pin")]
        public async Task<IActionResult> PutAgent(int id, Location location)
        {
            Target? target = await _context.targets.FindAsync(id);
            if (target == null) { return StatusCode(StatusCodes.Status404NotFound); }
            else
            {
                _context.locations.Add(location);
                target.Location = location;
                target.LocationId = location.Id;
                await _context.SaveChangesAsync();
                await _creatMissionByTarget.SearchTargetToTarget(target);// בדיקה האם יש מטרה קרובה
                _creatMissionByTarget.DeleteOldTasks();// מחיקת הצעות לא רלוונטיות
                return Ok(target);
            }
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> PutAgent(int id, DirectionModel direction)
        {
            Target? target = await _context.targets.FindAsync(id);
            if (target == null) { return NotFound("agent is null"); }
            else
            {
                Location? location = await _context.locations.FindAsync(target.LocationId);
                int test_x = location.x;
                int test_y = location.y;
                if(target.Status == StatusTarget.dead) { return NotFound("the target is ded"); }
                LoctionMeneger.ChangeLocation(location, direction.direction);
                //בודקת האם המטרה נדרשת לצאת לחלוטין מחוץ לגבולות המטריצה ומחזיר הודעת שגיעה
                if (test_x == location.x && test_y == location.y)
                {
                    return NotFound("An agent cannot go outside the matrix");
                }
                _context.Update(location);
                await _context.SaveChangesAsync();
                await _creatMissionByTarget.SearchTargetToTarget(target);// בדיקה האם יש מטרה קרובה
                _creatMissionByTarget.DeleteOldTasks();// מחיקת הצעות לא רלוונטיות
                return Ok();

            }
        }
    }
}
