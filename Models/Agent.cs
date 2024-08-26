using System.ComponentModel.DataAnnotations;

namespace MosadApi.Models
{
    public class Agent
    {
        [Key]
        public int Id { get; set; }
        public string photoUrl { get; set; }
        public string nickname { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        public StatusAgent? Status { get; set; }
        public string? token { get; set; }
    }
}

