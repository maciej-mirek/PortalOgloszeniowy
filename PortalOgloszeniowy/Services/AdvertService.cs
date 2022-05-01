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

        public List<Advert> GetUsersAdverts(ApplicationUser user)
        {
            return _db.Adverts.Where(a => a.User.Id == user.Id).ToList();
        }

        public IQueryable<Advert> GetAdvertsByCategory(int CategoryId)
        {
            var adverts = _db.Adverts.Where(a => a.CategoryId == CategoryId)
                .OrderBy(a => a.Created_at)
                .OrderByDescending(a => a.isPremium);
            
            
            return adverts;
        } 

        public Advert? GetAdvertUrl(string slug)
        {
            return _db.Adverts.Where(a => a.slug == slug).FirstOrDefault();
        }

        public void ViewsIncrementation(Advert advert)
        {
            advert.ViewsCount++; 
            _db.Adverts.Update(advert);
            _db.SaveChanges();
        }

        public List<Advert> SearchAdvertsByPhrase(string value)
        {
            var adverts = _db.Adverts.Where(a => a.Title.Contains(value))
                .OrderByDescending(a => a.isPremium)
                .ToList();

            return adverts;
        }

        public List<Advert> GetPremiumAdverts()
        {
            var adverts = _db.Adverts.Where(a => a.isPremium == true)
                .OrderBy(x => Guid.NewGuid())
                .Take(4)
                .ToList();

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
