using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalOgloszeniowy.Models.ViewModels
{
    public class AdvertViewModel
    {
        public Advert Advert  { get; set; }

        [ValidateNever]
        public IEnumerable<SelectListItem>? CategoryDropDown { get; set; }

        [ValidateNever]
        public List<AdvertImages> Images { get; set; }

    }
}
