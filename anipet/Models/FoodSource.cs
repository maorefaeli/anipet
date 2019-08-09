using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anipet.Models
{
    public class FoodSource
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter food source name")]
        [Display(Name = "Food source name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter food source price per kilo (in Shekels)")]
        [Display(Name = "Food source price per kilo (in Shekels)")]
        [Range(1, 10000)]
        public int SourcePricePerKilo { get; set; }

        public ICollection<Product> ProductsFromFoodSource { get; set; }
    }
}
