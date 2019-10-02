using System.ComponentModel.DataAnnotations;

namespace MagaWishlist.Models
{
    public class AuthViewModel
    {
        [Required]
        public string Username { get; set; }

        [Required]
        public string AccessKey { get; set; }
    }
}
