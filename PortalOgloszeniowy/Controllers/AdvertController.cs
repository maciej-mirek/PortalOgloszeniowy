using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PortalOgloszeniowy.Models;
using PortalOgloszeniowy.Models.ViewModels;
using Vereyon.Web;

namespace PortalOgloszeniowy.Controllers
{
    public class AdvertController : Controller
    {
        UserManager<ApplicationUser> _userManager;
        ApplicationDbContext _db;
        IFlashMessage _flashMessage;
        public AdvertController(ApplicationDbContext db, UserManager<ApplicationUser> userManager,
            IFlashMessage flashMessage)
        {
            _db = db;
            _userManager = userManager;
            _flashMessage = flashMessage;
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
                model.Advert.Created_at = DateTime.Now;
                model.Advert.User = await _userManager.GetUserAsync(User);
                await _db.Adverts.AddAsync(model.Advert);
                await _db.SaveChangesAsync();
                _flashMessage.Confirmation("Dodano nowe ogłoszenie.");
                return RedirectToAction("Index", "Home");
            }

            
            return View(model);
        }
       


        [Route("/advert/{id}")]
        public ActionResult Get(int id)
        {
            return Ok(id);
        }
    }
}
