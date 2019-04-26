using System.Collections.Generic;

namespace Aiplugs.CMS.Web.ViewModels
{
    public class UserViewModel
    {
        public IEnumerable<IUser> List { get; set; }
        public IUser User { get; set; }
    }
}