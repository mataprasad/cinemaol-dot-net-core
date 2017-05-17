using Microsoft.AspNetCore.Http;

namespace WebApplication.Models
{
    public class MovieInfo
    {
        public IFormFile fuPoster { get; set; }
        public int Movie_Id { get; set; }

        public string Movie_ImageURL { get; set; }

        public int Movie_Status { get; set; }
        public string MovieStatus_Value { get; set; }

        public string Movie_Title { get; set; }

        public string Movie_ReleaseDate { get; set; }

        public string Movie_Director { get; set; }

        public string Movie_Casts { get; set; }

        public string Movie_Language { get; set; }

        public string Movie_Industry { get; set; }

    }

}