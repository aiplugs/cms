using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Aiplugs.CMS.Web.Models;
using Aiplugs.CMS.Web.ViewModels;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Data.Entities;

namespace Aiplugs.CMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        const string SAMPLE_USER = "sample@local";
        const string SAMPLE_PASS = "P@ssw0rd";
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var signin = await _signInManager.PasswordSignInAsync(SAMPLE_USER, SAMPLE_PASS, false, lockoutOnFailure: false);
            if (signin.Succeeded == false)
            {
                var user = new User { UserName = SAMPLE_USER, Email = SAMPLE_USER };
                var signup = await _userManager.CreateAsync(user, SAMPLE_PASS);

                if (signup.Succeeded == false)
                    throw new InvalidOperationException("Cannot register sample user.");

                await _signInManager.SignInAsync(user, isPersistent: false);
            }
            
            return RedirectToLocal(returnUrl);
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction(nameof(DefaultController.Index), "Default");
            }
        }
    }
}
