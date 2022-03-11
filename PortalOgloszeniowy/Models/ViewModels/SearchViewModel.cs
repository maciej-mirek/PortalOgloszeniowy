using Microsoft.AspNetCore.Mvc.Rendering;

namespace PortalOgloszeniowy.Models.ViewModels
{
    public class SearchViewModel
    {
        public string? SearchPhrase { get; set; }
        public IEnumerable<SelectListItem>? CategoryDropDown { get; set; }

    }
}
