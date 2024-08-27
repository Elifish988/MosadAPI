using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Meneger;
using MosadApi.Models;
using System.Reflection;

namespace MosadApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class missionsController : ControllerBase
    {
        private readonly MosadDBContext _context;
        private readonly MissionsMeneger _missionsMeneger;
        public missionsController(MosadDBContext mosadDBContext, MissionsMeneger missionsMeneger)
        {
            _context = mosadDBContext;
            _missionsMeneger = missionsMeneger;
        }


        [HttpGet]
        public async Task<IActionResult> GetMissions()
        {
            var missions = await _context.missoions.ToListAsync();
            return Ok(missions);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetMissionsByID(int id)
        {
            Missoion? missoion = await _context.missoions.FindAsync(id);
            return Ok(missoion);
        }


        [HttpPut("{id}")]
        public async Task<IActionResult> assignedMissoions(int id, StatusMissoion statusMissoion)
        {
            Missoion? missoion = await _context.missoions.FindAsync(id);
            Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
            Target? target = await _context.targets.FindAsync(missoion.TargetId);
            await _missionsMeneger.DeleteOldTasks();// מחיקת מטרה במקרה שהיא התרחקה מאז ההצעה 
            if (missoion == null || await _missionsMeneger.DeleteIfIsNotRelevant(missoion) == false) return NotFound("The mission is not relevant anymore");
            missoion.Status = StatusMissoion.assigned;
            missoion.timeToDo = await _missionsMeneger.TimeToKill(agent, target);
            agent.Status = StatusAgent.active;
            await _context.SaveChangesAsync();
            return Ok(missoion.Status);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissions()
        {
            var m = await _context.missoions.Include(mission => mission.Agent)
                .ThenInclude(Agent => Agent.Location).Include(mission => mission.Target).ThenInclude(Target => Target.Location).ToListAsync();
            foreach (Missoion missoion in m)
            {
                if (missoion.Status == StatusMissoion.assigned)
                {
                    // בדיקה האם המטרה והסוכן חולקים משבצת
                    if ( _missionsMeneger.theyAreTogether(missoion.Agent.Location, missoion.Target.Location))
                    {
                        _missionsMeneger.Kill(missoion.Agent, missoion.Target, missoion);
                        return Ok("Objective successfully completed");
                    }
                    else
                    {
                        // מחשב זמן עד לחיסול
                        missoion.timeToDo = await _missionsMeneger.TimeToKill(missoion.Agent, missoion.Target);
                        // בודק את מיקום המטרה ביחס לסוכן ומקדם אות אליה
                        await _missionsMeneger.Direction(missoion.Agent.Location, missoion.Target.Location);

                        return Ok("An agent advanced to the target");

                    }

                }
                return Ok("Inactive target");
            }
            return Ok("Passed all missoions");
        }




    }
}
