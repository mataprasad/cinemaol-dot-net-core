using System.IO;
using Microsoft.AspNetCore.Http;

namespace WebApplication.Services
{
    public class GlobalOption
    {
        public GlobalOption()
        {
        }
        public string DefaultConnectionString { get; set; }
        public string ContentRootPath { get; set; }

        public bool SaveFile(IFormFile file, string name)
        {

            using (var fileS = new FileStream(Path.Combine(this.ContentRootPath, "wwwroot/images/movieImages/" + name), FileMode.Create))
            {
                file.CopyTo(fileS);
            }
            return true;
        }
    }
}
