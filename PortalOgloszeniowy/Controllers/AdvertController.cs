using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOgloszeniowy.Models;
using PortalOgloszeniowy.Models.ViewModels;
using PortalOgloszeniowy.Services;
using Slugify;
using Vereyon.Web;

namespace PortalOgloszeniowy.Controllers
{
    public class AdvertController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        ApplicationDbContext _db;
        IFlashMessage _flashMessage;
        IAdvertService _advertService;
        SlugHelper _slugger;
        public AdvertController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            IFlashMessage flashMessage, IAdvertService advertService, SlugHelper slugHelper)
        {
            _db = db;
            _userManager = userManager;
            _flashMessage = flashMessage;
            _advertService = advertService;
            _slugger = slugHelper;
        }


        [Authorize]
        [Route("/create")]
        public ActionResult Create()
        {

            AdvertViewModel model = new AdvertViewModel()
            {
                Advert = new Advert(),
                CategoryDropDown = _db.Categories.Select(c => new SelectListItem {
                    Text = c.Name,
                    Value = c.Id.ToString()
                })
            };

            return View(model);
        }

        [Authorize]
        [HttpPost]
        [Route("/create")]
        public async Task<ActionResult> CreateAsync(AdvertViewModel model)
        {

            if (ModelState.IsValid)
            {
                model.Advert.slug=_slugger.GenerateSlug(model.Advert.Title);
                model.Advert.Created_at = DateTime.Now;
                model.Advert.User = await _userManager.GetUserAsync(User);
                await _db.Adverts.AddAsync(model.Advert);
                await _db.SaveChangesAsync();
                _flashMessage.Confirmation("Dodano nowe ogłoszenie.");
                return RedirectToAction("Index", "Home");
            }

            model.CategoryDropDown = _db.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
  
                return View(model);
            

            
            
        }

        [Route("/{category}")]
        public ActionResult Category(string category)
        {
            var cat = _db.Categories.Where(c => c.Name == category).FirstOrDefault();
            if (cat == null)
                return NotFound();

            ViewBag.Adverts = _advertService.GetAdvertsByCategory(cat.Id);
            return Ok();
        }

        [Route("/advert/{slug}")]
        public ActionResult SingleAdvert(string slug)
        {
            var advert = _advertService.GetAdvertUrl(slug);

            if (advert == null)
                return NotFound();

            return Ok(advert);
        }

   
    }
}
