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
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdvertController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            IFlashMessage flashMessage, IAdvertService advertService, SlugHelper slugHelper, IWebHostEnvironment webHostEnvironment)
        {
            _db = db;
            _userManager = userManager;
            _flashMessage = flashMessage;
            _advertService = advertService;
            _slugger = slugHelper;
            _webHostEnvironment = webHostEnvironment;
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
        public async Task<ActionResult> CreateAsync(AdvertViewModel model, IFormFile? file)
        {

            if (ModelState.IsValid)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                if (file != null)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(wwwRootPath, @"images");
                    var extension = Path.GetExtension(file.FileName);

                    using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                         file.CopyTo(fileStreams);
                    }
                    model.Advert.ImageUrl = @"\images" + fileName + extension;
                }

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
        public async Task<ActionResult> Category(string category)
        {
            var cat = _db.Categories.Where(c => c.Name == category).FirstOrDefault();
            if (cat == null)
                return NotFound();

            var adv = _advertService.GetAdvertsByCategory(cat.Id);
            ViewBag.Adverts = adv;

           // var test = await PaginationList<Advert>.CreateAsync((IQueryable<Advert>)adv,1,5);
            var test2 = await PaginationList<Advert>.CreateAsync(_db.Adverts,1,5);

            return View(test2);
        }

        [Route("/advert/{slug}")]
        public ActionResult SingleAdvert(string slug)
        {
            var advert = _advertService.GetAdvertUrl(slug);

            if (advert == null)
                return NotFound();

            _advertService.ViewsIncrementation(advert);

            return Ok(advert);
        }

        [Route("adverts/{slug}")]
        public ActionResult SearchAdverts(string slug)
        {
            var adverts = _advertService.SearchAdvertsByPhrase(slug);

            ViewBag.Adverts = adverts;

            return View(adverts);
        }



    }
}
