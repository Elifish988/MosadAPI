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
            foreach (Agent agent in _context.agents)
            {
                if (await IsNear(agent, target) && await IsVacant(agent))
                {
                    await CreateMissoion(agent, target);
                }

            }
        }

        public async Task<bool> IsVacant(Agent agent)
        {
            await foreach (Missoion missoion in _context.missoions)
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

