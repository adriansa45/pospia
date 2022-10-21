using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class UserLoginViewModel
    {
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
       [Display(Name ="Remember Me")]
        public bool RememberMe { get; set; } = true;
    
    }
}
