using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Models;
using System.Reflection;

namespace MosadApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class missionsController : ControllerBase
    {
        private readonly MosadDBContext _context;
        public missionsController(MosadDBContext mosadDBContext)
        {
            _context = mosadDBContext;
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
            if (missoion == null) { return StatusCode(StatusCodes.Status404NotFound); }
            missoion.Status = statusMissoion;
            _context.SaveChanges();
            return Ok(missoion.Status);
        }
    }
}
