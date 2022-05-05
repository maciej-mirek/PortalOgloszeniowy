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
        ISlugHelper _slugger;
        IUploadImageService _uploadImageService;
        const int AdvertsOnPage = 5;
        public AdvertController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            IFlashMessage flashMessage, IAdvertService advertService, ISlugHelper slugHelper, 
            IUploadImageService uploadImageService)
        {
            _db = db;
            _userManager = userManager;
            _flashMessage = flashMessage;
            _advertService = advertService;
            _slugger = slugHelper;
            _uploadImageService = uploadImageService;
        }


        [Authorize]
        [Route("/create")]
        public ActionResult Create()
        {
            // Wyrzucic to ?????????
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
        public async Task<ActionResult> Create(AdvertViewModel model, IFormFile? file,
            IFormFileCollection? files, decimal price)
        {

            if (ModelState.IsValid)
            {

                
                if (file is not null)
                    model.Advert.ImageUrl = _uploadImageService.AdvertBanner(file);

                if (files is not null)
                    _uploadImageService.AdvertImages(files, model.Advert);


                model.Advert.slug=_slugger.GenerateSlug(model.Advert.Title);
                model.Advert.User = await _userManager.GetUserAsync(User);
                model.Advert.Price = price;

                 await _db.Adverts.AddAsync(model.Advert);
                 await _db.SaveChangesAsync();
                _flashMessage.Confirmation("Dodano nowe ogłoszenie.");
                return RedirectToAction("Index", "Home");
            }

            if(ModelState["Advert.CategoryId"]?.Errors.Count > 0)
            {
                ModelState["Advert.CategoryId"]?.Errors.Clear();
                ModelState["Advert.CategoryId"]?.Errors.Add("Musisz wybrać kategorie dla ogłoszenia.");


            }
            if (ModelState["price"]?.Errors.Count > 0)
            {
                ModelState["Advert.Price"]?.Errors.Clear();
                ModelState["Advert.Price"]?.Errors.Add("Wprowadz cene w formacie: 12,34");

            }

            model.CategoryDropDown = _db.Categories.Select(c => new SelectListItem
            {
                Text = c.Name,
                Value = c.Id.ToString()
            });
  
                return View(model);

        }

        [Authorize]
        public ActionResult EditAdvert(int? id)
        {

            //dodac walidacje uzytkownika :3 //

            if (id is null || id == 0)
            {
                return NotFound();
            }

            var adv = _db.Adverts.Find(id);
            if (adv is null)
            {
                return NotFound();
            }

            AdvertViewModel model = new AdvertViewModel()
            {
                Advert = adv,
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public ActionResult EditAdvert(AdvertViewModel model, decimal price)
        {

            if (ModelState.IsValid)
            {
                model.Advert.Price = price;
                _advertService.EditAdvert(model.Advert);
                _flashMessage.Confirmation("Edytowano ogłoszenie.");
                return RedirectToAction("Profile", "Account");

            }

            if (ModelState["price"]?.Errors.Count > 0)
            {
                ModelState["Advert.Price"]?.Errors.Clear();
                ModelState["Advert.Price"]?.Errors.Add("Wprowadz cene w formacie: 12,34");

            }

            return View(model);
        }


        [Authorize]
        [Route("Account/Advert/ModalDeleteAdvert/{id}")]
        public ActionResult ModalDeleteAdvert(int id)
        {
            var advert = _db.Adverts.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("_ModalAdvertDeletePartial", advert);
        }


        [HttpPost]
        [Authorize]
        public async Task<ActionResult> DeleteAdvert(int advertId)
        {

            var user = await _userManager.GetUserAsync(User);
            
            var advert = _db.Adverts.Find(advertId);
            if(advert?.User != user)
            {
                return NotFound();
            }

            if (!_advertService.DeleteAdvert(advertId))
                return NotFound();

            _flashMessage.Info("Usunięto ogłoszenie.");

            return RedirectToAction("Profile","Account");

        }


        [Authorize]
        [Route("Account/Advert/Premium/{id}")]
        public ActionResult PremiumAdvertModal(int id)
        {
            var advert = _db.Adverts.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("_ModalAdvertPremiumPartial", advert);
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult> PremiumAdvert(int? advertId)
        {

            var user = await _userManager.GetUserAsync(User);

            var advert = _db.Adverts.Find(advertId);
            if (advert.User != user)
            {
                return NotFound();
            }

            if (advertId is null)
            {
                _flashMessage.Warning("Błąd przy płatności.");
                return RedirectToAction("Profile", "Account");
            }

            if (!_advertService.PremiumAdvert(advert))
            {
                _flashMessage.Warning("Wystąpił problem.");
                return RedirectToAction("Profile", "Account");
            }

            _flashMessage.Confirmation("Aktywowano pakiet premium dla ogłoszenia.");

            return RedirectToAction("Profile", "Account");

        }


        [Route("/{category}")]
        public async Task<ActionResult> Category(string category, int pageNumber)
        {
            var cat = _db.Categories.Where(c => c.Name == category).FirstOrDefault();
            if (cat is null)
                return NotFound();

            var adv = _advertService.GetAdvertsByCategory(cat.Id);

           pageNumber = pageNumber == 0 ? 1 : pageNumber;
           var pagination = PaginationService<Advert>.CreateAsync(adv, pageNumber, AdvertsOnPage);
           int pageCount = adv.Count() / AdvertsOnPage;
           pageCount = adv.Count() % AdvertsOnPage != 0 ? pageCount + 1 : pageCount;

           ViewBag.pageCount = pageCount;

            return View(pagination);
        }

        [Route("/advert/{slug}")]
        public ActionResult SingleAdvert(string slug)
        {
            var advert = _advertService.GetAdvertUrl(slug);

            if (advert is null)
                return NotFound();

            _advertService.ViewsIncrementation(advert);

            ViewBag.Advert = advert;
            ViewBag.AdvertOwner = _db.Users.Find(advert.UserId);
            ViewBag.Gallery = _db.AdvertImages.Where(a => a.Advert.Id == advert.Id).ToList();

            return View(advert);
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
