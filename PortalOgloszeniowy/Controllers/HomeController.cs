using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOgloszeniowy.Models;
using PortalOgloszeniowy.Models.ViewModels;
using PortalOgloszeniowy.Services;
using System.Diagnostics;

namespace PortalOgloszeniowy.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _db;
        IAdvertService _advertService;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext db,
            IAdvertService advertService)
        {
            _logger = logger;
            _db = db;
            _advertService = advertService;
        }

        public IActionResult Index()
        {
            SearchViewModel model = new SearchViewModel()
            {
                CategoryDropDown = _db.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            var premium = _advertService.GetPremiumAdverts();

            ViewBag.PremiumAdverts = premium;

            return View();
        }

        [HttpPost]
        public IActionResult Index(SearchViewModel model)
        {

            if(ModelState.IsValid)
            {
                if(model.SearchPhrase is not null)
                {
                    return Redirect("/adverts/"+model.SearchPhrase);
                }
                
            }

            var premium = _advertService.GetPremiumAdverts();

            ViewBag.PremiumAdverts = premium;


            SearchViewModel mod = new SearchViewModel()
            {
                CategoryDropDown = _db.Categories.Select(c => new SelectListItem
                {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };


            return View(mod);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}