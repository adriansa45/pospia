using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class Profile
    {
        public int UserId { get; set; }
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public string LastName { get; set; }
        public decimal Balance { get; set; }
        public decimal Salary { get; set; }
        [Range(minimum:1,maximum:365)]
        public int IntervalDays { get; set; }
        public DateTime NextPay { get; set; }
    }
}
