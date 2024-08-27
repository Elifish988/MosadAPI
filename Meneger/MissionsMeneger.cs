using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using MosadApi.DAL;
using MosadApi.Models;
using System.Reflection;
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
            missoion.Agent = agent;
            missoion.Target = target;
            missoion.Status = StatusMissoion.Offer;
            missoion.timeToDo = await TimeToKill(agent, target);
            _context.missoions.Add(missoion);
            await _context.SaveChangesAsync();
        }


        // בודקת האם ההצעות הקודמות רלוונטיות
        public async Task DeleteOldTasks()
        {
            var mis = await _context.missoions.Include(mission => mission.Agent)
                .Include(mission => mission.Target).ToListAsync();
            foreach (Missoion missoion in mis)
            {
                if (missoion.Status == StatusMissoion.Offer)
                {
                    
                    if (!await IsNear(missoion.Agent, missoion.Target))
                    {
                        _context.missoions.Remove(missoion);
                        await _context.SaveChangesAsync();
                    }
                }

            }
        }



        // מחיקת הצעה כאשר מטרה או סוכן אינם פנויים
        public async Task<bool> DeleteIfIsNotRelevant(Missoion missoion)
        {
            
            if (missoion.Agent.Status != StatusAgent.Dormant)
            {
                // טלאי שנועד למחיקת משימות שלא תהיינה רלוונטיות לעולם
                if(missoion.Status == 0)
                {
                    _context.missoions.Remove(missoion);
                    _context.SaveChanges();
                }
                return false;
            }
            else
            {
                var mis1 = await _context.missoions.ToArrayAsync();
                foreach (Missoion mis in mis1)
                {
                    if(mis.TargetId == missoion.Target.Id && mis.Status != StatusMissoion.Offer)
                    {
                        // טלאי שנועד למחיקת משימות שלא תהיינה רלוונטיות לעולם
                        if (missoion.Status == 0)
                        {
                            _context.missoions.Remove(missoion);
                            _context.SaveChanges();
                        }
                        return false;
                    }
                }
            }
            return true;

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
            agent.CountKill += 1;
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
        // צריך להעביר למנג'ר ייחודי
        //פונקציה להחזרת מידה על מטרות עבור MVC
        public async Task<List<MissionsMVC>> GetOptions()
        {
            List<MissionsMVC> missionsMVCs = new List<MissionsMVC>();
            // טלאי קצת מכוער - לטיפול בהמשך
            // מחיקת מטרה במקרה שהיא התרחקה מאז ההצעה
            await DeleteOldTasks();
            //מחיקת הצעה במקרה והמטרה או הסוכן נתפסו
           var m = await _context.missoions
                .Include(mission => mission.Agent).ThenInclude(Agent => Agent.Location)
                .Include(mission => mission.Target).ThenInclude(Target => Target.Location).ToListAsync();
            foreach (Missoion missoion in m)
            {
                if (missoion.Status == StatusMissoion.Offer && await DeleteIfIsNotRelevant(missoion))
                {
                    MissionsMVC missionsMVC = new MissionsMVC();
                    missionsMVC.Id = missoion.Id;
                    missionsMVC.Agent = missoion.Agent.nickname;
                    missionsMVC.AgentLocation = 
                        $"x: {missoion.Agent.Location.x} , y: {missoion.Agent.Location.y}";
                    missionsMVC.Target = missoion.Target.name;
                    missionsMVC.TargetLocation = 
                        $"x: {missoion.Target.Location.x} , y: {missoion.Target.Location.y}";
                    missionsMVC.Distance = await HowFar(missoion.Agent, missoion.Target);
                    missionsMVC.Executiontime = missoion.Executiontime;
                    missionsMVCs.Add(missionsMVC);

                }
            }
            return missionsMVCs;
        }


        // מייצר את כל הסכימות עבור GeneralView
        public async Task<GeneralView> GeneralView()
        {
            GeneralView generalView = new GeneralView();
            generalView.SumAgent = await _context.agents.CountAsync();
            generalView.SumAgentActiv = await _context.agents.CountAsync(agent => agent.Status == StatusAgent.active);
            generalView.SumTarget = await _context.targets.CountAsync();
            generalView.SumTargetActiv = await _context.targets.CountAsync(target => target.Status == StatusTarget.free);
            generalView.SumMissions = await _context.missoions.CountAsync();
            generalView.SumMissionsActiv = await _context.missoions.CountAsync(mission => mission.Status == StatusMissoion.Offer);
            generalView.agentsToTarget = generalView.SumAgent / generalView.SumTarget;
            generalView.agentsToTargetRelevant = await _context.agents.CountAsync
                (agent => _context.missoions.Any
                (mission => mission.Status == 0 &&
                mission.AgentId == agent.Id &&
                agent.Status == StatusAgent.Dormant));

            return generalView;
        }


        // מייצר את כל הסכימות עבור GeneralView
        public async Task<List<AgentStatusMVC>> AgentStatus()
        {
            List<AgentStatusMVC> agentStatusMVCs = new List<AgentStatusMVC>();
            var As = await _context.agents.Include(Agent => Agent.Location).ToListAsync();
            foreach(Agent agent in As)
            {
                AgentStatusMVC agentStatusMVC = new AgentStatusMVC();
                agentStatusMVC.Name = agent.nickname;
                agentStatusMVC.Locition = $" X : {agent.Location.x} , Y {agent.Location.y}";
                agentStatusMVC.Status = agent.Status.ToString();
                Missoion? mission = await _context.missoions.FirstOrDefaultAsync(mission => mission.AgentId == agent.Id && mission.Status == StatusMissoion.assigned);
                if(mission != null)
                {
                    agentStatusMVC.mission = mission.Id;
                    agentStatusMVC.timeToDo = mission.timeToDo;
                }
                agentStatusMVC.CountKills = agent.CountKill;
                agentStatusMVCs.Add(agentStatusMVC);

            }

            return agentStatusMVCs;


        }
        


        // מייצר את כל הסכימות עבור GeneralView
        public async Task<List<TargetStatusMVC>> TargetStatus()
        {
            List<TargetStatusMVC> targetStatusMVCs = new List<TargetStatusMVC>();
            var Ts = await _context.targets.Include(Target => Target.Location).ToListAsync();
            foreach (Target target in Ts)
            {
                TargetStatusMVC targetStatusMVC = new TargetStatusMVC();
                targetStatusMVC.name = target.name;
                targetStatusMVC.Location = $" X : {target.Location.x} , Y {target.Location.y}";
                targetStatusMVC.Status = target.Status.ToString();
                targetStatusMVC.position = target.position;
                targetStatusMVCs.Add(targetStatusMVC);

            }
            return targetStatusMVCs;
        }
    }
}
