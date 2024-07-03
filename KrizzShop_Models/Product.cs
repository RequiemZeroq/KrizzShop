using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KrizzShop_Models
{
    public class Product
    {
        public Product()
        {
            TempQuantity = 1;
        }

        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string ShortDesc { get; set; }
        public string Description { get; set; } 
        [Range(1, int.MaxValue)]
        public double Price { get; set; }
        public string Image { get; set; }
        [Display(Name = "Category Type")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public virtual Category Category { get; set; }
        [Display(Name = "Application Type")]
        public int ApplicationTypeID { get; set; }
        [ForeignKey("ApplicationTypeID")]
        public virtual ApplicationType ApplicationType { get; set; }
        [NotMapped]
        [Range(1, 10000, ErrorMessage = "Quantity must be greater than 0 and less than 10000")]
        public int TempQuantity { get; set; }   
    }
}
