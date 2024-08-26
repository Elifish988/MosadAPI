using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MosadApi.DAL;
using MosadApi.Meneger;
using MosadApi.Models;

namespace MosadApi.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class MVCController : ControllerBase
    {
        private readonly MosadDBContext _context;
        private readonly MissionsMeneger _missionsMeneger;
        public MVCController(MosadDBContext mosadDBContext, MissionsMeneger missionsMeneger)
        {
            _context = mosadDBContext;
            _missionsMeneger = missionsMeneger;
        }

        // מחזירה את רשימת כל המשימות להצעה עבור MVC
        [HttpGet("GetOptions")]
        public async Task<IActionResult> GetOptions()
        {
            try
            {
                List<MissionsMVC> missionsMVCs = await _missionsMeneger.GetOptions();
                return Ok(missionsMVCs);
            }
            catch
            {
                return NotFound("");
            }
        }




        // מייצר את כל הסכימות עבור GeneralView
        [HttpGet("GeneralView")]
        public async Task<IActionResult> GeneralView()
        {
            try
            {
                GeneralView generalView = await _missionsMeneger.GeneralView();
                return Ok(generalView);
            }
            catch
            {
                return NotFound("generalView not found");
            }



        }

        //מייצר את כל הסכימות עבור AgentStatus
        [HttpGet("AgentStatus")]
        public async Task<IActionResult> AgentStatus()
        {
            try
            {
                List<AgentStatusMVC> agentStatusMVCs = await _missionsMeneger.AgentStatus();
                return Ok(agentStatusMVCs);
            }
            catch
            {
                return NotFound("AgentStatus not found");
            }
        }



        //מייצר את כל הסכימות עבור TargetStatus
        [HttpGet("TargetStatus")]
        public async Task<IActionResult> TargetStatus()
        {
            try
            {
                List<TargetStatusMVC> targetStatusMVCs = await _missionsMeneger.TargetStatus();
                return Ok(targetStatusMVCs);
            }
            catch
            {
                return NotFound("TargetStatus not found");
            }
        }
    }
}
