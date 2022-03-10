using PortalOgloszeniowy.Models;

namespace PortalOgloszeniowy.Services
{
    public interface IAdvertService
    {
        public List<Advert> GetAdverts();

        public List<Advert> GetUsersAdverts(ApplicationUser user);

        public List<Advert> GetAdvertsByCategory(int CategoryId);

        public Advert GetAdvertUrl(string slug);


    }
}
