using Microsoft.AspNetCore.Http.HttpResults;
using MosadApi.DAL;
using MosadApi.Models;
using System.Security.Cryptography;

namespace MosadApi.Meneger
{
    public  class MissionsMeneger
    {
        private readonly MosadDBContext _context;
        public MissionsMeneger(MosadDBContext mosadDBContext)
        {
            _context = mosadDBContext;
        }

        // בודקת האם הסוכן בטווח המטרה
        protected async Task<bool> IsNear(Agent agent , Target target)
        {
            Location? agentLocation = await _context.locations.FindAsync(agent.LocationId);
            Location? targetLocation = await _context.locations.FindAsync(target.LocationId);
            if (Math.Sqrt( Math.Pow(targetLocation.x - agentLocation.x, 2) + Math.Pow(targetLocation.y - agentLocation.y, 2)) <= 200)
            {
                return true;
            }
            return false;
        }



        // מייצרת מטרה
        protected async Task CreateMissoion(Agent agent, Target target)
        {
            Missoion missoion = new Missoion();
            missoion.TargetId = target.Id;
            missoion.AgentId = agent.Id;
            missoion.Status = StatusMissoion.Offer;
            _context.missoions.Add(missoion);
            _context.SaveChanges();
        }


        // בודקת האם ההצעות הקודמות רלוונטיות
        protected async Task DeleteOldTasks()
        {
            foreach(Missoion missoion in _context.missoions)
            {
                Target? target = await _context.targets.FindAsync(missoion.TargetId);
                Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
                if (! await IsNear(agent, target))
                {
                    _context.missoions.Remove(missoion);
                }
            }
        }
    }
}
