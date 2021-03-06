using PortalOgloszeniowy.Models;

namespace PortalOgloszeniowy.Services
{
    public interface IAdvertService
    {


        public List<Advert> GetUsersAdverts(ApplicationUser user);

        public List<Advert> GetAdvertsByCategory(int CategoryId);

        public Advert? GetAdvertUrl(string slug);

        public void ViewsIncrementation(Advert advert);

        public List<Advert> SearchAdvertsByPhrase(string value);

        public List<Advert> GetPremiumAdverts();

        public bool DeleteAdvert(int? id);

        public void EditAdvert(Advert advert);

        public bool PremiumAdvert(Advert advert);


    }
}
