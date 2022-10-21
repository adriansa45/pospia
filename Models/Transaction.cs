using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public decimal Amount { get; set; }   
        [Required]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;
        [Display(Name ="Category")]
        public int Category { get; set; }
    }
}
