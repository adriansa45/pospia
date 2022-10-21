using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class Category
    {
        [Required]
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Color { get; set; }
    }
}
