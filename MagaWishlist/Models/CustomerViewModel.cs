using System.ComponentModel.DataAnnotations;

namespace MagaWishlist.Models
{
    public class CustomerViewModel
    {
        public string Id { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        public string Name { get; set; }
    }
}
