using Microsoft.AspNetCore.Http.HttpResults;
using MosadApi.DAL;
using MosadApi.Models;
using System.Security.Cryptography;

namespace MosadApi.Meneger
{
    public class MissionsMeneger
    {
        private readonly MosadDBContext _context;
        public MissionsMeneger(MosadDBContext mosadDBContext)
        {
            _context = mosadDBContext;
        }

        // מה המרחק מהמטרה
        public async Task<Double> HowFar(Agent agent, Target target)
        {
            Location? agentLocation = await _context.locations.FindAsync(agent.LocationId);
            Location? targetLocation = await _context.locations.FindAsync(target.LocationId);

            return Math.Sqrt(Math.Pow(targetLocation.x - agentLocation.x, 2) + Math.Pow(targetLocation.y - agentLocation.y, 2));
        }

        // בודקת האם הסוכן בטווח המטרה
        protected async Task<bool> IsNear(Agent agent, Target target)
        {
            
            if ( await HowFar(agent, target) <= 200)
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
        public async Task DeleteOldTasks()
        {
            foreach (Missoion missoion in _context.missoions)
            {
                if (missoion.Status == StatusMissoion.Offer)
                {
                    Target? target = await _context.targets.FindAsync(missoion.TargetId);
                    Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
                    if (!await IsNear(agent, target))
                    {
                        _context.missoions.Remove(missoion);
                        _context.SaveChanges();
                    }
                }

            }
        }



        // מחיקת הצעה כאשר מטרה או סוכן אינם פנויים
        public async Task DeleteIfIsNotRelevant(Missoion missoion)
        {
            Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
            Target? target = await _context.targets.FindAsync(missoion.TargetId);
            if (agent.Status != StatusAgent.Dormant)
            {
                _context.missoions.Remove(missoion);
                _context.SaveChanges();
            }
            else
            {
                foreach(Missoion mis in _context.missoions)
                {
                    if(mis.TargetId == target.Id && mis.Status != StatusMissoion.Offer)
                    {
                        _context.missoions.Remove(missoion);
                        _context.SaveChanges();
                    }
                }
            }

        }

        // בודק האם הסוכן והמטרה נמצאים באותו המיקום
        public bool theyAreTogether(Location agentLocation, Location targetLocation)
        {
            if (agentLocation.x == targetLocation.x && agentLocation.y == targetLocation.y)
            { return true; }
            return false;
        }


        // חיסול של מטרה
        public async Task Kill(Agent agent, Target target, Missoion missoion)
        {
            agent.Status = StatusAgent.Dormant;
            target.Status = StatusTarget.dead;
            _context.missoions.Remove(missoion);
            _context.SaveChanges();
        }

        //בדיקה מה הזמן שנותר עד חיסול המטרה
        public async Task<Double> TimeToKill(Agent agent, Target target)
        {
            return await HowFar(agent, target) / 5;
        }

        // מחשבת את כיוון המטרה
        public async Task<Direction> Direction(Location agentLocation, Location targetLocation)
        {
            string tmp = "";
            if(agentLocation.x < targetLocation.x)
            {
                tmp += "e";
            }
            else if (agentLocation.x > targetLocation.x)
            {
                tmp += "w";
            }
            if (agentLocation.y < targetLocation.y)
            {
                tmp += "s";
            }
            else if (agentLocation.y > targetLocation.y)
            {
                tmp += "n";
            }

            for()
        }
    }
}
