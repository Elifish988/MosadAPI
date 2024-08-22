using System.ComponentModel.DataAnnotations;

namespace MosadApi.Models
{
    public class Agent
    {
        [Key]
        public Guid? Id { get; set; }
        public string photo_url { get; set; }
        public string Name { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        public StatusAgent? Status { get; set; }
    }
}

