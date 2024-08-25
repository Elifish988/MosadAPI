namespace MosadApi.Models
{
    public class MissionsMVC
    {
        public int Id { get; set; }
        public string Agent { get; set; }
        public string AgentLocation { get; set; }
        public string Target { get; set; }
        public string TargetLocation { get; set; }
        public double Distance { get; set; }
        public DateTime? Executiontime { get; set; }
    }
}
