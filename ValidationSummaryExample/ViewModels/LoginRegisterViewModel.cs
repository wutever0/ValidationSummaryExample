using ValidationSummaryExample.FormModels;

namespace ValidationSummaryExample.ViewModels
{
    public class LoginRegisterViewModel
    {
        public LoginFormModel loginFormModel { get; set; }

        public RegisterFormModel registerFormModel { get; set; }

        public string ReturnUrl { get; set; }
    }
}
