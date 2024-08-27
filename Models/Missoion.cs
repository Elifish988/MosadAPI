using System.ComponentModel.DataAnnotations;

namespace MosadApi.Models
{
    public class Missoion
    {
        [Key]
        public int Id { get; set; }
        public int AgentId { get; set; }
        public int TargetId { get; set; }
        public Agent Agent { get; set; }
        public Target Target { get; set; }
        public Double? timeToDo { get; set; }
        public StatusMissoion Status { get; set; }
        public DateTime? Executiontime { get; set; }
        public string? token { get; set; }
    }
}
