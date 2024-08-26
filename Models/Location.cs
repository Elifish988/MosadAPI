using System.ComponentModel.DataAnnotations;

namespace MosadApi.Models
{
    public class Location
    {
        [Key]
        public int Id { get; set; }
        [Range(0,1001)]
        public int x { get; set; }
        [Range(0, 1001)]
        public int y { get; set; }
        public Location(int x, int y)
        {
            this.x = x;
            this.y = y;
        }
        public Location() { }

    }
}
