using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalOgloszeniowy.Models
{
    public class Advert
    {
        [Key]
        public int Id { get; set; }

        public string Title { get; set; }

        public string Description { get; set; }

        public string Location { get; set; }

        public decimal? Price { get; set; }

        public string? slug { get; set; }

        public DateTime Created_at { get; set; }

        public string UserId { get; set; }

        [ForeignKey("UserId")]
        public ApplicationUser User{ get; set; }


        [ValidateNever]
        public int CategoryId { get; set; }


        [ValidateNever]
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public int ViewsCount { get; set; } = 0;

        public bool isPremium { get; set; } = false;

        [ValidateNever]
        public string? ImageUrl { get; set; }

    }
}
