using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PortalOgloszeniowy.Models
{
    public class Advert
    {

        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        
        public int Id { get; set; }

        [Required(ErrorMessage = "Tytuł jest wymagany.")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Opis jest wymagany.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Lokalizacja jest wymagana.")]
        public string Location { get; set; }

        //[RegularExpression(@"^[1-9]\d{0,7}(?:\,\d{1,4})?$",ErrorMessage ="Wprowadz cene w formacie: 12,34")]
        // [ValidateNever]
        //[DataType(DataType.Currency,ErrorMessage ="test")]
        public decimal Price { get; set; }

        public string? slug { get; set; }

        public DateTime Created_at { get; set; }
        
        [ValidateNever]
        public string UserId { get; set; }

        [ForeignKey("UserId")]
        [ValidateNever]
        public ApplicationUser User{ get; set; }


      
        public int CategoryId { get; set; }


        
        [ForeignKey("CategoryId")]
        public virtual Category? Category { get; set; }

        public int ViewsCount { get; set; } = 0;

        public bool isPremium { get; set; } = false;

        [ValidateNever]
        public string? ImageUrl { get; set; }

    }
}
