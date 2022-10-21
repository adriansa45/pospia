using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class TransactionViewModel: Transaction
    {
        public IEnumerable<SelectListItem> Categories;
        public int Months { get; set; } = 1;
    }
}
