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

        public IQueryable<Advert> GetAdvertsByCategory(int CategoryId)
        {
            var adverts = _db.Adverts.Where(a => a.CategoryId == CategoryId);

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

        public List<Advert> SearchAdvertsByPhrase(string value)
        {
            var adverts = _db.Adverts.Where(a => a.Title.Contains(value)).ToList();

            return adverts;
        }

        public List<Advert> GetPremiumAdverts()
        {
            var adverts = _db.Adverts.Where(a => a.isPremium == true).ToList();

            return adverts;
        }

        public bool DeleteAdvert(int? id)
        {
            var adv = _db.Adverts.Find(id);
            if(adv != null)
            {
                _db.Adverts.Remove(adv);
                _db.SaveChanges();
                return true;
            }
            return false;

        }

        public void EditAdvert(Advert advert)
        {

            advert.User = _db.Users.Find(advert.UserId);
            advert.Category = _db.Categories.Find(advert.CategoryId);
            _db.Adverts.Update(advert);
            _db.SaveChanges();
        }

        public bool PremiumAdvert(Advert advert)
        {
            if (advert != null)
            {
                advert.isPremium = true;
                _db.Adverts.Update(advert);
                _db.SaveChanges();
                return true;
            }
            return false;
        }
    }
}
