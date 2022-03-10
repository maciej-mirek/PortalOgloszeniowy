using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalOgloszeniowy.Models.ViewModels
{
    public class AdvertViewModel
    {
        public Advert Advert  { get; set; }
        public IEnumerable<SelectListItem>? CategoryDropDown { get; set; }

    }
}
