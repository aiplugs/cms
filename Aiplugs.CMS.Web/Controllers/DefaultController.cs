using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Aiplugs.CMS.Web.ViewModels;
using Aiplugs.CMS.Web.Filters;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace Aiplugs.CMS.Web.Controllers
{
    [ServiceFilter(typeof(SharedDataLoad))]
    public class DefaultController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        // [HttpGet("/login")] 
        // [AllowAnonymous]
        // public IActionResult Login()
        // {
        //     return View();
        // }

        // [HttpPost("/login")] 
        // [AllowAnonymous]        
        // public async Task<IActionResult> Login(LoginViewModel model)
        // {
        //     var claims = new List<Claim>() 
        //     { 
        //         new Claim(ClaimTypes.Name,"aiplugs::cms")
        //     };

        //     var userPrincipal = new ClaimsPrincipal(new ClaimsIdentity(claims, "DummyLogin"));

        //     await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal, new AuthenticationProperties 
        //     { 
        //         ExpiresUtc = DateTime.UtcNow.AddMinutes(20), 
        //         IsPersistent = false, 
        //         AllowRefresh = true 
        //     }); 

        //     return RedirectToAction("Index", "Home"); 
        // }

        // [HttpGet("/logout")]
        // [AllowAnonymous]        
        // public async Task<IActionResult> Logout()
        // {
        //     await HttpContext.SignOutAsync("Cookie"); 
 
        //     return RedirectToAction("Index", "Home"); 
        // }

        // [HttpGet("/forbidden")] 
        // [AllowAnonymous]        
        // public IActionResult Forbidden()
        // {
        //     return new StatusCodeResult(403);
        // }
    }
}
