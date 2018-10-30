using System.ComponentModel.DataAnnotations;

namespace ValidationSummaryExample.FormModels
{
    public class LoginFormModel
    {
        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [DataType(DataType.Password)]
        [Required]
        public string Password { get; set; }

        [Display(Name = "Remember me?")]
        public bool RememberMe { get; set; }
    }
}
