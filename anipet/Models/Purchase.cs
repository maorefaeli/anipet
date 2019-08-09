using System;
using System.ComponentModel.DataAnnotations;

namespace anipet.Models
{
    public class Purchase
    {
        public int Id { get; set; }

        public int PurchaseId { get; set; }

        [Required(ErrorMessage = "Please enter Username")]
        public User User { get; set; }

        [Required(ErrorMessage = "Please enter product")]
        public Product Product { get; set; }

        public DateTime Date { get; set; }
    }
}
