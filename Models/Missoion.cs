using System.ComponentModel.DataAnnotations;

namespace MosadApi.Models
{
    public class Missoion
    {
        [Key]
        public Guid Id { get; set; }
        public Guid AgentId { get; set; }
        public Guid TargetId { get; set; }
        public Double? timeToDo { get; set; }
        public StatusMissoion Status { get; set; }
        public DateTime? Executiontime { get; set; }
    }
}
