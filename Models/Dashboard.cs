namespace POS.Models
{
    public class Dashboard
    {
        public decimal Balance { get; set; }
        public decimal Bills { get; set; }
        public decimal EarnMonthly { get; set; }
        public decimal EarnAnual { get; set; }
        public decimal BillsSubscriptions { get; set; }
        public decimal NextMonth { get; set; }
        public IEnumerable<Transaction> LastTrasactions { get; set; }
        public IEnumerable<Subscription> Subscriptions { get; set; }
    }
}
