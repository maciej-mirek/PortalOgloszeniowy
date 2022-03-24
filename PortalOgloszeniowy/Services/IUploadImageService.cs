using PortalOgloszeniowy.Models;

namespace PortalOgloszeniowy.Services
{
    public interface IUploadImageService
    {
        public string AdvertBanner(IFormFile file);
        public void AdvertImages(IFormFileCollection files, Advert advert);

    }
}
