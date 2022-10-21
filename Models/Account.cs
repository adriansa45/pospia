using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class Account
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public decimal Salary { get; set; }
        [DataType(DataType.Date)]
        public DateTime PayDay { get; set; } = DateTime.Today;
        public int Interval_Days { get; set; }

    }
}
