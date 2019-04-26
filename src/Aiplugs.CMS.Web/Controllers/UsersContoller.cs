using System.Threading.Tasks;
using Aiplugs.CMS.Web.Attributes;
using Aiplugs.CMS.Web.Filters;
using Aiplugs.CMS.Web.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Aiplugs.CMS.Web.Controllers
{
    [ServiceFilter(typeof(SharedDataLoad))]
    public class UsersController : Controller
    {
        private readonly IUserManageService _service;
        public UsersController(
            IUserManageService service
        ) {
            _service = service;
        }

        [HttpGet("/settings/users/")]
        public async Task<IActionResult> List()
        {
            var model = new UserViewModel
            {
                List = await _service.GetUsersAsync()
            };

            return View(model);
        }

        [HttpGet("/settings/users/{id}")]
        public async Task<IActionResult> Item([FromRoute]string id, [FromHeader(Name = "X-IC-Request")]bool isIntercooler)
        {
            var user = await _service.FindUserAsync(id);

            if (user == null)
                return NotFound();

            if (isIntercooler)
                return View(user);

            var model = new UserViewModel
            {
                List = await _service.GetUsersAsync(),
                User = user
            };

            return View(nameof(List), model);
        }

        [HttpPost("/settings/users/{id}")]
        [SubmitAction("delete")]
        public async Task<IActionResult> Remove([FromRoute]string id)
        {
            var user = await _service.FindUserAsync(id);

            if (user == null)
                return NotFound();
            
            await _service.RemoveUserAsync(user.Id);

            return RedirectToAction(nameof(List));
        }
    }
}