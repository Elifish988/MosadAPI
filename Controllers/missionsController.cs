using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Meneger;
using MosadApi.Models;
using System.Reflection;

namespace MosadApi.Controllers
{
    [Route("api/[controller]")]
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


        [HttpPut("{id}")]
        public async Task<IActionResult> assignedMissoions(int id, StatusMissoion statusMissoion)
        {
            Missoion? missoion = await _context.missoions.FindAsync(id);
            Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
            Target? target = await _context.targets.FindAsync(missoion.TargetId);
            await _missionsMeneger.DeleteOldTasks();// מחיקת מטרה במקרה שהיא התרחקה מאז ההצעה
            await _missionsMeneger.DeleteIfIsNotRelevant(missoion);// מחיקת הצעה במקרה והמטרה או הסוכן נתפסו 
            if (missoion == null) return NotFound("The mission is not relevant anymore");
            missoion.Status = statusMissoion;
            missoion.timeToDo = await _missionsMeneger.TimeToKill(agent, target);
            agent.Status = StatusAgent.active;
            _context.SaveChanges();
            return Ok(missoion.Status);
        }

        [HttpPost("update")]
        public async Task<IActionResult> UpdateMissions()
        {
            foreach (Missoion missoion in _context.missoions)
            {
                if (missoion.Status == StatusMissoion.assigned)
                {
                    Target? target = await _context.targets.FindAsync(missoion.TargetId);
                    Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
                    Location? agentLocation = await _context.locations.FindAsync(agent.LocationId);
                    Location? targetLocation = await _context.locations.FindAsync(target.LocationId);
                    if (_missionsMeneger.theyAreTogether(agentLocation, targetLocation))// בדיקה האם המטרה והסוכן חולקים משבצת
                    {
                        _missionsMeneger.Kill(agent, target, missoion);
                        return Ok("Objective successfully completed"); 
                    }
                    else
                    {
                        missoion.timeToDo = await _missionsMeneger.TimeToKill(agent, target);


                    }
                }
            }
        }
    }
}
