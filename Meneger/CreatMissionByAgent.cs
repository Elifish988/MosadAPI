using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Models;
using MosadApi.Controllers;

namespace MosadApi.Meneger
{
    public class CreatMissionByAgent: MissionsMeneger
    {
        private readonly MosadDBContext _context;

        public CreatMissionByAgent(MosadDBContext mosadDBContext) : base(mosadDBContext)
        {
            _context = mosadDBContext;
        }

        public async Task SearchTargetToAgent(Agent agent)
        {
            var tar = await _context.targets.ToArrayAsync();
            foreach (Target target in tar)
            {
                if (await IsNear(agent, target) && await IsVacant(target))
                {
                    CreateMissoion(agent, target);
                }

            }
        }

        public async Task<bool> IsVacant(Target target)
        {
            var mis = await _context.missoions.ToArrayAsync();
            foreach (Missoion missoion in mis)
            {
                if (missoion.TargetId == target.Id && missoion.Status != StatusMissoion.Offer)
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
