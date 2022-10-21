using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class Subscription
    {
        public int Id { get; set; }
        public int AccountId { get; set; }
        [Required]
        [StringLength(50)]
        public string Name { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime Date { get; set; } = DateTime.Now;
        [DataType(DataType.Date)]
        public DateTime LastPay { get; set; } = DateTime.Today;
        public int IntervalDay { get; set; } = 30;
        [DataType(DataType.Date)]
        public DateTime NextPay { get; set; } = DateTime.Today.AddMonths(1);

    }
}
