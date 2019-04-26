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
using Aiplugs.CMS.Data;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Web.Controllers
{
    [Authorize]
    [Route("[controller]/[action]")]
    public class AccountController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AiplugsDbContext _db;
        private readonly ILogger _logger;

        public AccountController(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            RoleManager<Role> roleManager,
            AiplugsDbContext db,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _db = db;
            _logger = logger;
        }

        [HttpGet("/account/admin")]
        [AllowAnonymous]
        public async Task<IActionResult> Admin()
        {
            var count = await _db.Users.CountAsync();

            if (count > 0)
                return Forbid();

            return View(new AdminViewModel());
        }

        [HttpPost("/account/admin")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateAdmin([FromForm]AdminViewModel model)
        {
            if (!ModelState.IsValid)
                return View(nameof(Admin), model);

            var count = await _db.Users.CountAsync();
            if (count > 0)
                return Forbid();

            var user = new User { UserName = model.DisplayName, Email = model.Email };
            var result = await _userManager.CreateAsync(user, model.Password);

            if (!result.Succeeded)
            {
                ViewData["Errors"] = result.Errors.Select(err => err.Description);
                return View(nameof(Admin), model);
            }

            await _userManager.AddToRoleAsync(user, "admin");

            await _signInManager.SignInAsync(user, isPersistent: false);

            return RedirectToAction("Index", "Default");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToLocal(returnUrl);
        
            return View(new LoginViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromForm] LoginViewModel model, [FromQuery]string returnUrl = null)
        {
            if (!ModelState.IsValid)
                return View(nameof(Login), model);

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            var signin = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
            if (!signin.Succeeded)
            {
                return View(nameof(Login), new LoginViewModel());
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
