using PortalOgloszeniowy.Models;

namespace PortalOgloszeniowy.Services
{
    public class UploadImageService : IUploadImageService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;
        private readonly ApplicationDbContext _db;
        string wwwRootPath;
        string uploads;


        public UploadImageService(IWebHostEnvironment webHostEnvironment, ApplicationDbContext db)
        {
            _webHostEnvironment = webHostEnvironment;
            wwwRootPath = _webHostEnvironment.WebRootPath;

            uploads = Path.Combine(wwwRootPath, @"images");
            _db = db;
        }

        public string AdvertBanner(IFormFile file)
        {
            string fileName = Guid.NewGuid().ToString();
            var extension = Path.GetExtension(file.FileName);

            using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
            {
                file.CopyTo(fileStreams);
            }
            return @"\images\" + fileName + extension;

        }

        public void AdvertImages(IFormFileCollection files,Advert advert)
        {
            List<AdvertImages> images = new List<AdvertImages>();   

            foreach (var f in files)
            {
                string fileName = Guid.NewGuid().ToString();
                var extension = Path.GetExtension(f.FileName);

                using (var fileStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                {
                    f.CopyTo(fileStreams);
                }
                AdvertImages advertImage = new AdvertImages()
                {
                    ImageUrl = @"\images\" + fileName + extension,
                    Advert = advert,
                };
                _db.AdvertImages.AddAsync(advertImage);
            }

        }
    }
}
