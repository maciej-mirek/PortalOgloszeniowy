using System.ComponentModel.DataAnnotations;

namespace PortalOgloszeniowy.Models
{
    public class Advert
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public string Location { get; set; }

        public double? Price { get; set; }

        public string slug { get; set; }

        public DateTime Created_at { get; set; }

        public ApplicationUser User{ get; set; }
    }
}
