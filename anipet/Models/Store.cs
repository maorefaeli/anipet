using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anipet.Models
{
    public class Store
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Store must have Manager, please enter store manager")]
        public User StoreManager { get; set; }

        public ICollection<Product> Products { get; set; }

        [Required(ErrorMessage = "Please enter the store city")]
        public string City { get; set; }

        [Required(ErrorMessage = "Please enter the store street")]
        [Display(Name = "Street")]
        public string StreetAddress { get; set; }
    }
}
