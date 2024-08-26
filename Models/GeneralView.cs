namespace MosadApi.Models
{
    public class GeneralView
    {
        public int? SumAgent { get; set; }
        public int? SumAgentActiv { get; set; }
        public int? SumTarget { get; set; }
        public int? SumTargetActiv { get; set; }
        public int? SumMissions { get; set; }
        public int? SumMissionsActiv { get; set; }
        public double? agentsToTarget { get; set; }
        public int? agentsToTargetRelevant { get; set; }
    }
}
