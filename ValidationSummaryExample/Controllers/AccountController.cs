using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ValidationSummaryExample.FormModels;
using ValidationSummaryExample.ViewModels;

namespace ValidationSummaryExample.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly ILogger _logger;
        private readonly IEmailSender _emailSender;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            ILogger<AccountController> logger,
            IEmailSender emailSender)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
        }

        [TempData]
        public string ErrorMessage { get; set; }


        // GET: Account
        [AllowAnonymous]
        [HttpGet]
        //[Route("/LoginRegister")]
        public async Task<ActionResult> LoginRegister(string returnUrl)
        {
            if (!string.IsNullOrEmpty(ErrorMessage)) {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            LoginRegisterViewModel viewModel = new LoginRegisterViewModel();

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            viewModel.ReturnUrl = returnUrl;

            return View(viewModel);
        }

        // GET: Account
        [AllowAnonymous]
        [HttpPost]
        //[Route("/Login")]
        public async Task<ActionResult> Login(string returnUrl, LoginFormModel loginFormModel)
        {
            returnUrl = returnUrl ?? Url.Content("~/");

            if (ModelState.IsValid) {
                // This doesn't count login failures towards account lockout
                // To enable password failures to trigger account lockout, set lockoutOnFailure: true
                var result = await _signInManager.PasswordSignInAsync(loginFormModel.Email, loginFormModel.Password, loginFormModel.RememberMe, lockoutOnFailure: true);
                if (result.Succeeded) {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect(returnUrl);
                }
                if (result.RequiresTwoFactor) {
                    return RedirectToPage("./LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginFormModel.RememberMe });
                }
                if (result.IsLockedOut) {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("./Lockout");
                }
                else {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return View("LoginRegister", new LoginRegisterViewModel { ReturnUrl = returnUrl });
                }
            }

            // If we got this far, something failed, redisplay form
            return View("LoginRegister", new LoginRegisterViewModel { ReturnUrl = returnUrl });
        }

        // GET: Account
        [HttpPost]
        //[Route("/Register")]
        public async Task<ActionResult> Register(string returnUrl, RegisterFormModel registerFormModel)
        {
            returnUrl = returnUrl ?? Url.Content("~/");
            if (ModelState.IsValid) {
                var user = new IdentityUser { UserName = registerFormModel.Email, Email = registerFormModel.Email };
                var result = await _userManager.CreateAsync(user, registerFormModel.Password);
                if (result.Succeeded) {
                    _logger.LogInformation("User created a new account with password.");

                    var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                    var callbackUrl = Url.Page(
                        "/Account/ConfirmEmail",
                        pageHandler: null,
                        values: new { userId = user.Id, code = code },
                        protocol: Request.Scheme);

                    await _emailSender.SendEmailAsync(registerFormModel.Email, "Confirm your email",
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");

                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return LocalRedirect(returnUrl);
                }
                foreach (var error in result.Errors) {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            LoginRegisterViewModel viewModel = new LoginRegisterViewModel();
            viewModel.ReturnUrl = returnUrl;

            // If we got this far, something failed, redisplay form
            return View("LoginRegister", viewModel);
        }

    }
}