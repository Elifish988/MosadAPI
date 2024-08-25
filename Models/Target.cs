
using global::MosadApi.Models;
using System.ComponentModel.DataAnnotations;

namespace MosadApi.Models
{
    public class Target
    {
        [Key]
        public int Id { get; set; }
        public string name { get; set; }
        public string photo_url { get; set; }
        public string position { get; set; }
        public int? LocationId { get; set; }
        public Location? Location { get; set; }
        public StatusTarget? Status { get; set; }

    }
}
