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
        public async Task<ActionResult> Create(AdvertViewModel model, IFormFile? file, IFormFileCollection? files)
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
                    model.Advert.ImageUrl = @"\images\" + fileName + extension;
                }


                model.Advert.slug=_slugger.GenerateSlug(model.Advert.Title);
                model.Advert.Created_at = DateTime.Now;
                model.Advert.User = await _userManager.GetUserAsync(User);


                if (files != null)
                {
                    foreach (var f in files)
                    {
                        string fileName = Guid.NewGuid().ToString();
                        var uploads = Path.Combine(wwwRootPath, @"images");
                        var extension = Path.GetExtension(f.FileName);

                        using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                        {
                            f.CopyTo(fileStreams);
                        }
                        AdvertImages advertImages = new AdvertImages()
                        {
                            ImageUrl = @"\images\" + fileName + extension,
                            Advert = model.Advert,
                        };
                         await _db.AdvertImages.AddAsync(advertImages);
                    }
                }



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


        public ActionResult EditAdvert(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var adv = _db.Adverts.Find(id);
            if (adv == null)
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
        public ActionResult EditAdvert(AdvertViewModel model)
        {

            if (ModelState.IsValid)
            {
                _advertService.EditAdvert(model.Advert);
                _flashMessage.Confirmation("Edytowano ogłoszenie.");
                return RedirectToAction("Profile", "Account");

            }
            return View(model);
        }


        [HttpGet]
        [Route("Account/Advert/ModalAdvert/{id}")]
        public ActionResult ModalAdvert(int id)
        {
            var advert = _db.Adverts.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("_ModalAdvertDeletePartial", advert);
        }



        [Route("Advert/DeleteAdvert/{advertId}")]
        [Authorize]
        public async Task<ActionResult> DeleteAdvert(int? advertId)
        {

            var user = await _userManager.GetUserAsync(User);
            
            var advert = _db.Adverts.Find(advertId);
            if(advert.User != user)
            {
                return NotFound();
            }

            if (!_advertService.DeleteAdvert(advertId))
                return NotFound();

            _flashMessage.Info("Usunięto ogłoszenie.");

            return RedirectToAction("Profile","Account");

        }




        [HttpGet]
        [Route("Account/Advert/Premium/{id}")]
        public ActionResult PremiumAdvertModal(int id)
        {
            var advert = _db.Adverts.Where(a => a.Id == id).FirstOrDefault();
            return PartialView("_ModalAdvertPremiumPartial", advert);
        }


        [Route("Advert/PremiumAdvert/{advertId}")]
        [Authorize]
        public async Task<ActionResult> PremiumAdvert(int? advertId)
        {

            var user = await _userManager.GetUserAsync(User);

            var advert = _db.Adverts.Find(advertId);
            if (advert.User != user)
            {
                return NotFound();
            }

            if (advertId == null)
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
            if (cat == null)
                return NotFound();

            var adv = _advertService.GetAdvertsByCategory(cat.Id);

           

           pageNumber = pageNumber == 0 ? 1 : pageNumber;
           var pagination = await PaginationList<Advert>.CreateAsync(adv, pageNumber, 5);
           int pageCount = adv.Count() / 5;
           pageCount = adv.Count() % 5 != 0 ? pageCount + 1 : pageCount;

           ViewBag.pageCount = pageCount;

            return View(pagination);
        }

        [Route("/advert/{slug}")]
        public ActionResult SingleAdvert(string slug)
        {
            var advert = _advertService.GetAdvertUrl(slug);

            if (advert == null)
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
