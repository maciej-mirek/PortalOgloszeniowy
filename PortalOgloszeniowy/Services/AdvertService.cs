using PortalOgloszeniowy.Models;

namespace PortalOgloszeniowy.Services
{
    public class AdvertService : IAdvertService
    {

        ApplicationDbContext _db;
        public AdvertService(ApplicationDbContext db)
        {
            _db = db;
        }
        public List<Advert> GetAdverts()
        {
            var adverts = _db.Adverts.ToList();

            return adverts;
        }

        public List<Advert> GetUsersAdverts(ApplicationUser user)
        {
            var adverts = _db.Adverts.Where(a => a.User.Id == user.Id).ToList();

            return adverts;
        }

        public List<Advert> GetAdvertsByCategory(int CategoryId)
        {
            var adverts = _db.Adverts.Where(a => a.CategoryId == CategoryId).ToList();

            return adverts;
        }

        public Advert GetAdvertUrl(string slug)
        {
            var advert = _db.Adverts.Where(a => a.slug == slug).FirstOrDefault();

            return advert;
        }

        public void ViewsIncrementation(Advert advert)
        {
            advert.ViewsCount++; 
            _db.Adverts.Update(advert);
            _db.SaveChanges();
        }
    }
}
