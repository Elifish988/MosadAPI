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
            foreach (Target target in _context.targets)
            {
                if (await IsNear(agent, target) && await IsVacant(target))
                {
                    CreateMissoion(agent, target);
                }

            }
        }

        public async Task<bool> IsVacant(Target target)
        {
            await foreach (Missoion missoion in _context.missoions)
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
