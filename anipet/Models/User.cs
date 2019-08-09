using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anipet.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter password")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please choose if this is a Admin or not")]
        public bool IsAdmin { get; set; }

        [Required(ErrorMessage = "Please enter Username")]
        [Display(Name = "Username")]
        public string Username { get; set; }

        public ICollection<Store> Stores { get; set; }

        public Product FavoriteProduct { get; set; }
    }
}
