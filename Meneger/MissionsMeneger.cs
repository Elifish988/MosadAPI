using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
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
            if(agent == null || target == null) { return 30000; }
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
            missoion.timeToDo = await TimeToKill(agent, target);
            _context.missoions.Add(missoion);
            await _context.SaveChangesAsync();
        }


        // בודקת האם ההצעות הקודמות רלוונטיות
        public async Task DeleteOldTasks()
        {
            var mis = await _context.missoions.ToArrayAsync();
            foreach (Missoion missoion in mis)
            {
                if (missoion.Status == StatusMissoion.Offer)
                {
                    Target? target = await _context.targets.FindAsync(missoion.TargetId);
                    Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
                    if (!await IsNear(agent, target))
                    {
                        _context.missoions.Remove(missoion);
                        await _context.SaveChangesAsync();
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
                await _context.SaveChangesAsync();
            }
            else
            {
                var mis1 = await _context.missoions.ToArrayAsync();
                foreach (Missoion mis in mis1)
                {
                    if(mis.TargetId == target.Id && mis.Status != StatusMissoion.Offer)
                    {
                        _context.missoions.Remove(missoion);
                        await _context.SaveChangesAsync();
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
            await _context.SaveChangesAsync();
        }

        //בדיקה מה הזמן שנותר עד חיסול המטרה
        public async Task<Double> TimeToKill(Agent agent, Target target)
        {
            return await HowFar(agent, target) / 5;
        }

        // מחשבת את כיוון המטרה
        public async Task Direction(Location agentLocation, Location targetLocation)
        {
            
            if(agentLocation.x < targetLocation.x)
            {
                agentLocation.x += 1;
            }
            if (agentLocation.x > targetLocation.x)
            {
                agentLocation.x -= 1;
            }
            if (agentLocation.y < targetLocation.y)
            {
                agentLocation.y += 1;
            }
            if (agentLocation.y > targetLocation.y)
            {
                agentLocation.y -= 1;
            }
            _context.Update(agentLocation);
            await _context.SaveChangesAsync();

        }

        //פונקציה להחזרת מידה על מטרות עבור MVC
        public async Task<List<MissionsMVC>> GetOptions()
        {
            List<MissionsMVC> missionsMVCs = new List<MissionsMVC>();
            // טלאי קצת מכוער - לטיפול בהמשך
            // מחיקת מטרה במקרה שהיא התרחקה מאז ההצעה
            await DeleteOldTasks();
            //מחיקת הצעה במקרה והמטרה או הסוכן נתפסו
           var m = await _context.missoions.ToListAsync();
            foreach (var Missoion in m)
            {
                await DeleteIfIsNotRelevant(Missoion);
            }
            foreach (Missoion missoion in m)
            {
                if (missoion.Status == StatusMissoion.Offer)
                {
                    MissionsMVC missionsMVC = new MissionsMVC();
                    Target? target = await _context.targets.FindAsync(missoion.TargetId);
                    Agent? agent = await _context.agents.FindAsync(missoion.AgentId);
                    Location? agentLocation = await _context.locations.FindAsync(agent.LocationId);
                    Location? targetLocation = await _context.locations.FindAsync(target.LocationId);
                    missionsMVC.Id = missoion.Id;
                    missionsMVC.Agent = agent.Name;
                    missionsMVC.AgentLocation = $"x: {agentLocation.x} , y: {agentLocation.y}";
                    missionsMVC.Target = target.name;
                    missionsMVC.TargetLocation = $"x: {targetLocation.x} , y: {targetLocation.y}";
                    missionsMVC.Distance = await HowFar(agent, target);
                    missionsMVC.Executiontime = missoion.Executiontime;
                    missionsMVCs.Add(missionsMVC);

                }
            }
            return missionsMVCs;
        }

    }
}
