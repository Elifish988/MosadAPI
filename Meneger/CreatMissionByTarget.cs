using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Models;

namespace MosadApi.Meneger
{
    public class CreatMissionByTarget: MissionsMeneger
    {
        private readonly MosadDBContext _context;

        public CreatMissionByTarget(MosadDBContext mosadDBContext) : base(mosadDBContext)
        {
            _context = mosadDBContext;
        }

        public async Task SearchTargetToTarget(Target target)
        {
            var age = await _context.agents.ToListAsync();
            foreach (Agent agent in age)
            {
                if (await IsNear(agent, target) && await IsVacant(agent))
                {
                    await CreateMissoion(agent, target);
                }

            }
        }

        public async Task<bool> IsVacant(Agent agent)
        {
            var mis = await _context.missoions.ToArrayAsync();
            foreach (Missoion missoion in mis)
            {
                if (missoion.AgentId == agent.Id && missoion.Status != StatusMissoion.Offer)
                {
                    return false;
                }

            }
            return true;
        }

        public void DeleteOldTasks()
        {
            base.DeleteOldTasks();
        }
    }
}

