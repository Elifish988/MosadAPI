﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosadApi.DAL;
using Microsoft.EntityFrameworkCore;
using MosadApi.Models;
using MosadApi.Helper;
using MosadApi.Models;
using MosadApi.Meneger;

namespace MosadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class agentsController : ControllerBase
    {

        private readonly MosadDBContext _context;
        private readonly CreatMissionByAgent _creatMissionByAgent;
        public agentsController(MosadDBContext mosadDBContext, CreatMissionByAgent creatMissionByAgent)
        {
            _context = mosadDBContext;
            _creatMissionByAgent = creatMissionByAgent;
        }

        



        [HttpGet]
        public async Task<IActionResult> GetAttacks()
        {
            var agents = await _context.agents.ToListAsync();
            return Ok(agents);
        }


        [HttpPost]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public IActionResult CreateAgent(Agent agent)
        {
            agent.Id = Guid.NewGuid();
            agent.Status = StatusAgent.Dormant;
            _context.agents.Add(agent);
            _context.SaveChanges();
            return Ok(agent.Id);
        }


        [HttpPut("{id}/pin")]
        public async Task<IActionResult> PutAgent(Guid id, Location location)
        {
            Agent? agent = await _context.agents.FindAsync(id);
            if (agent == null) { return StatusCode(StatusCodes.Status404NotFound); }
            else
            {
                _context.locations.Add(location);
                agent.Location = location;
                agent.LocationId = location.Id;
                _context.SaveChanges();
                await _creatMissionByAgent.SearchTargetToAgent(agent);// בדיקה האם יש מטרה קרובה
                _creatMissionByAgent.DeleteOldTasks();// מחיקת הצעות לא רלוונטיות
                return Ok(agent);
            }
        }

        [HttpPut("{id}/move")]
        public async Task<IActionResult> PutAgent(Guid id, Direction direction)
        {
            Agent? agent = await _context.agents.FindAsync(id);
            if (agent == null) { return NotFound("agent is null"); }
            if (agent.Status == StatusAgent.active) { return NotFound("agent is active"); }
            else
            {
                Location? location = await _context.locations.FindAsync(agent.LocationId);
                location = LoctionMeneger.ChangeLocation(location, direction);
                _context.Update(location);
                _context.SaveChanges();
                await _creatMissionByAgent.SearchTargetToAgent(agent);// בדיקה האם יש מטרה קרובה
                _creatMissionByAgent.DeleteOldTasks();// מחיקת הצעות לא רלוונטיות
                return Ok();

            }
        }

    }
}
