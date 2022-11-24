using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace POS.Models
{
    public class Product
    {
        
        public int ProductId { get; set; }
        [Display(Name = "Nombre")]
        public string Name { get; set; }
        [Display(Name = "Precio")]
        public decimal Price { get; set; }
        [Display(Name = "Cantidad")]
        public int Amount { get; set; }
        [Display(Name = "Imagen")]
        public string? Image { get; set; }
        [Display(Name = "Imagen")]
        public IFormFile? FormFile { get; set; }
        public int Available { get; set; }
    }
}
