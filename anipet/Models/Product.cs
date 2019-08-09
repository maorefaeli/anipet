using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace anipet.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Please enter product name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Product must have a food source, please select one")]
        public FoodSource FoodSource { get; set; }

        [Required(ErrorMessage = "Please enter Product weight")]
        [Display(Name = "Product weight in kilo")]
        [Range(1, 500)]
        public int ProductWeightInKilo { get; set; }

        public ICollection<Store> Stores { get; set; }

    }
}
