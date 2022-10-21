using System.ComponentModel.DataAnnotations;

namespace POS.Models
{
    public class UserViewModel: IValidatableObject
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string LastName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string CPassword { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Password != CPassword)
            {
                yield return new ValidationResult("Passwords have to be the same");
            }
        }
    }
}
